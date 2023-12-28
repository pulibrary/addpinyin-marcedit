using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AddPinyin
{

    public partial class frmMain : Form
    {
        private SortedDictionary<int, ChineseRecord> chineseRecords;
        private SortedDictionary<int, ChineseRecord> allRecords;
        
        private SortedSet<string> availableTags = new SortedSet<string>();
        private SortedSet<string> selectedTags = new SortedSet<string>();
        private List<string> defaultSelectedTags = new List<string>() {"1xx","2xx","3xx","4xx","50x","51x",
            "521","522","524","525","526","53x","54x","55x","56x","58x","59x","6xx","7xx","8xx"};
        
        private string cjkPattern = @"[\p{IsCJKCompatibility}\p{IsCJKUnifiedIdeographsExtensionA}\p{IsCJKUnifiedIdeographs}\p{IsCJKCompatibilityIdeographs}]";
        private string alphanumPattern = @"[\p{L}\p{N}\p{M}]";
        private romanizationDataSetTableAdapters.ChinesePinyinTableAdapter pinyinTableAdapter;
        private romanizationDataSet.ChinesePinyinDataTable pinyinTable;
        private int? maxChineseLen;
        public frmMain()
        { 
            InitializeComponent();
            System.Diagnostics.StackFrame[] frames = new System.Diagnostics.StackTrace().GetFrames();
            System.Reflection.Assembly initialAssembly = (from f in frames
                                                          select f.GetMethod().ReflectedType.Assembly
                         ).Last();
            try
            {
                FileInfo MarcEngineDLL = variables.meDir.GetFiles("mengine*").FirstOrDefault<FileInfo>();
                Assembly MarcEngineAssembly = Assembly.LoadFile(MarcEngineDLL.FullName);
                variables.MarcEngine = MarcEngineAssembly.GetType("mengine60.mengine60");
                variables.MEObject = Activator.CreateInstance(variables.MarcEngine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load MarcEdit libraries.\nPlease confirm that MarcEdit is installed.\n" + ex.ToString());
                Application.Exit();
            }            

            chineseRecords = new SortedDictionary<int, ChineseRecord>();
            allRecords = new SortedDictionary<int, ChineseRecord>();

            pinyinTableAdapter = new romanizationDataSetTableAdapters.ChinesePinyinTableAdapter();
            pinyinTable = pinyinTableAdapter.GetData();

            //maximum length of a Chinese character string found in Romanization table
            maxChineseLen = Convert.ToInt32(pinyinTableAdapter.MaxLenQuery());

            LoadChineseRecords();

            //display to the user how many records contain unconverted Chinese text

            if (chineseRecords.Count == 0)
            {
                this.labelStatus.Text = "No records were found with unconverted Chinese text.\n" +
                     "To swap existing parallel fields, check the box below.";
                this.whichFieldsLabel.Text = "";

            }
            else
            {
                this.labelStatus.Text = this.labelStatus.Text.Replace("#", chineseRecords.Count.ToString());
            }
            this.Show();

            //determine which specific fields contain unconverted Chinese text, and allow user to select which to convert.
            foreach (ChineseRecord rec in chineseRecords.Values)
            {
                List<string> recLines = rec.getLines();
                int n = recLines.Count;
                for (int i = 0; i < n; i++)
                {
                    string reci = recLines[i];
                    string tag = reci.Substring(1, 3);
                    if (!tag.Equals("LDR") && !tag.Substring(0, 2).Equals("00") && !tag.Equals("880"))
                    {
                        if (!reci.Contains("\u001F6") && Regex.IsMatch(reci, cjkPattern))
                        {
                            availableTags.Add(tag);
                        }
                    }
                }
            }

            List<string> dstNew = new List<string>();

            //explode wildcards in default tag names
            foreach (string defaultTag in defaultSelectedTags)
            {
                if (Regex.IsMatch(defaultTag, @"[0-9]xx"))
                {
                    string prefix = defaultTag.Substring(0, 1);
                    for (int i = 0; i <= 9; i++)
                    {
                        for (int j = 0; j <= 9; j++)
                        {
                            dstNew.Add(prefix + i + j);
                        }
                    }
                }
                else if (Regex.IsMatch(defaultTag, @"[0-9][0-9]x"))
                {
                    string prefix = defaultTag.Substring(0, 2);
                    for (int i = 0; i <= 9; i++)
                    {
                        dstNew.Add(prefix + i);
                    }
                }

            }
            defaultSelectedTags.AddRange(dstNew);

            //select certain tags by default
            foreach (string defaultTag in defaultSelectedTags)
            {

                if (availableTags.Contains(defaultTag))
                {
                    selectedTags.Add(defaultTag);
                    availableTags.Remove(defaultTag);
                }
            }
            refreshTagLists();
        }

        /*
         * swapParallelFields: swap the values of all the 880s and their corresponding fields
         * 
         */
        int swapParallelFields(bool pinyin880)
        {
            int recIndex = 0;
            int modRecCount = 0;
            SortedDictionary<int, ChineseRecord> convertedRecords = new SortedDictionary<int, ChineseRecord>();
            foreach (ChineseRecord rec in allRecords.Values)
            {
                //MessageBox.Show(rec.toString());
                bool modified = false;
                ChineseRecord newrec = new ChineseRecord();
                List<string> recLines = rec.getLines();
                int n = recLines.Count;
                Dictionary<int, string> fieldOriginals = new Dictionary<int, string>();
                Dictionary<int, string> field880s = new Dictionary<int, string>();
                for (int i = 0; i < n; i++)
                {
                    string reci = recLines[i];
                    string tag = reci.Substring(1, 3);
                    Match m = Regex.Match(reci, @"[\$\u001F]6[0-9]{3}-([0-9]{2})[^\$\u001F]*(.*)");

                    if (m.Success)
                    {
                        int seqno = int.Parse(m.Groups[1].Value);
                        string body = m.Groups[2].Value;
                        if (seqno > 0)
                        {
                            if ((tag != "880" && fieldOriginals.ContainsKey(seqno)) ||
                                tag == "880" && field880s.ContainsKey(seqno))
                            {
                                MessageBox.Show("Error in record #" + (recIndex+1) + ": The sequence number " +
                                    m.Groups[1].Value + " appears in subfield $6 of multiple field pairs. " +
                                    "Please resolve this problem and run the macro again.");
                                return 0;
                            }                            
                            if (tag == "880")
                            {
                                field880s[seqno] = body;
                            }
                            else
                            {
                                fieldOriginals[seqno] = body;
                            }
                        }
                    }
                }
                foreach (int seqno in fieldOriginals.Keys.Concat<int>(field880s.Keys))
                {
                    if (!fieldOriginals.ContainsKey(seqno) || !field880s.ContainsKey(seqno))
                    {
                        MessageBox.Show("Error in record #" + (recIndex+1) + ": The sequence number " +
                                    seqno.ToString("D2") + " only appears in one field with no parallel. " +
                                    "Please resolve this problem and run the macro again.");
                        return 0;
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    string reci = recLines[i];
                    string tag = reci.Substring(1, 3);
                    Match m = Regex.Match(reci, @".*[\$\u001F]6[0-9]{3}-([0-9]{2})[^\$\u001F]*");
                    if (m.Success)
                    {
                        string prefix = m.Groups[0].Value;
                        int seqno = int.Parse(m.Groups[1].Value);
                        if (seqno > 0 && pinyin880 == (Regex.IsMatch(field880s[seqno], cjkPattern) || field880s[seqno].Contains("{esc}{dollar}1")))
                        {
                            if (tag == "880")
                            {
                                newrec.addLine(prefix + fieldOriginals[seqno]);
                                modified = true;
                            }
                            else
                            {
                                newrec.addLine(prefix + field880s[seqno]);
                                modified = true;
                            }
                        }
                        else
                        {
                            newrec.addLine(reci);
                        }
                    }
                    else
                    {
                        newrec.addLine(reci);
                    }                   
                }
                if (modified)
                {
                    modRecCount++;
                }
                convertedRecords.Add(recIndex, newrec);
                recIndex++;

            }
            foreach (KeyValuePair<int, ChineseRecord> keyval in convertedRecords)
            {
                int index = keyval.Key;
                allRecords[index] = new ChineseRecord(convertedRecords[index].toString());
            }
            return modRecCount;
        }

        /*
         * LoadChineseRecords(): Loads all records into the allRecords data structure.  Records with Chinese characters outside 
         * of the 880 fields are loaded in the chineseRecords data structure, and converted if UTF-8 if needed.
         */
        private void LoadChineseRecords()
        {
            //display a dialog box letting the user know that the data is being read.
            loadingDialog loading = new loadingDialog();
            loading.Show();
            loading.Refresh();
            int recIndex = 0;
            string filename = variables.objEditor.Loaded_File;
            variables.objEditor.SaveFile(filename);

            StreamReader reader = variables.objEditor.ReadFile(filename);

            string line = "";
            string recStr = "";
            bool isMARC8 = false;
            bool isChinese = false;
            bool hasUnconvertedCJK = false;
            while (reader.Peek() > -1)
            {
                line = variables.objEditor.ReadLine(reader);
                if (line.Trim().Length > 0)
                {
                    try
                    {
                        string tag = line.Substring(1, 3);

                        //determine is record is MARC-8 or UTF-8 by looking at leader field.
                        if (tag.Equals("LDR") && line.Substring(15, 1).Equals(" "))
                        {
                            isMARC8 = true;
                        }

                        //determine if record is Chinese by looking at 008 field
                        if (tag.Equals("008"))
                        {
                            if (line.Substring(41, 3).Equals("chi"))
                            {
                                isChinese = true;
                            }
                        }

                        //determine if there are fields with Chinese characters but no linkage fields
                        if (!line.Contains("$6"))
                        {
                            if ((isMARC8 && line.Contains("{esc}{dollar}1")) ||
                                (!isMARC8 && Regex.IsMatch(line, cjkPattern)))
                            {
                                hasUnconvertedCJK = true;
                            }
                        }
                    }
                    catch (global::System.Exception ex)
                    {
                        MessageBox.Show("File contains malformed records.  Macro will terminate.\n" + ex.ToString());
                        Application.Exit();
                    }
                    recStr += line + Environment.NewLine;
                }

                if ((line.Trim().Length == 0 || reader.Peek() == -1) && recStr.Trim().Length > 0)
                {
                    if (isChinese && hasUnconvertedCJK)
                    {
                        recStr = recStr.Replace('$', '\u001F');
                        chineseRecords.Add(recIndex, new ChineseRecord(recStr));
                    }
                    allRecords.Add(recIndex, new ChineseRecord(recStr));
                    recIndex++;
                    isMARC8 = false;
                    isChinese = false;
                    hasUnconvertedCJK = false;
                    recStr = "";
                }
            }
            variables.objEditor.CloseFile(reader);
            loading.Hide();
        }

        /*
         * addPinyinToRecords(): Convert Chinese text to pinyin, insert 880 fields containing original text
         */
        private SortedDictionary<int, ChineseRecord> addPinyinToRecords()
        {
            SortedDictionary<int, ChineseRecord> newrecords = new SortedDictionary<int, ChineseRecord>();
            int numrecs = chineseRecords.Count;
            int recno = 0;
            bool addPinyinTo880 = field880radio.Checked;
            foreach (KeyValuePair<int, ChineseRecord> entry in chineseRecords)
            {
                recno++;
                HashSet<int> seqnos = new HashSet<int>();
                int seqno = 1;
                ChineseRecord rec = chineseRecords[entry.Key];
                List<string> lines = rec.getLines();
                int n = lines.Count;

                //determine which occurrence numbers are already used in the record
                for (int i = 0; i < n; i++)
                {
                    string li = lines[i];
                    Match m = Regex.Match(li, @"\u001F6[0-9]{3}-([0-9]{2})");
                    if (m.Success)
                    {
                        seqnos.Add(int.Parse(m.Groups[1].Value));
                    }
                }
                ChineseRecord newrec = new ChineseRecord();
                SortedDictionary<int, string> added880s = new SortedDictionary<int, string>();
                for (int i = 0; i < n; i++)
                {
                    string li = lines[i];
                    string tag = li.Substring(1, 3);
                    /*
                     * for each tag selected by user, check if it contains CJK text.  If so, convert to pinyin,
                     * and add 880 field with an unused occurrence number and original text
                     */

                    if (selectedTags.Contains(tag) && Regex.IsMatch(li, cjkPattern))
                    {
                        while (seqnos.Contains(seqno)) { seqno++; };
                        seqnos.Add(seqno);
                        string indicators = li.Substring(6, 2);
                        string indicatorsPinyin = indicators;
                        string cjkText = li.Substring(8);
                        string pinyinText = getPinyin(cjkText, tag, indicators);
                        string seqstr = "\u001F6" + tag + "-" + string.Format("{0:00}", seqno);
                        if (Convert.ToInt32(tag) >= 600 && Convert.ToInt32(tag) <= 651 && indicators.Substring(1, 1).Equals("4"))
                        {
                            indicatorsPinyin = indicators.Substring(0, 1) + "4";
                        }
                        if (addPinyinTo880)
                        {
                            added880s.Add(seqno, "=880  " + indicatorsPinyin + seqstr + pinyinText);
                            li = li.Substring(0, 6) + indicators + "\u001F6880-" + string.Format("{0:00}", seqno) + cjkText;
                        }
                        else
                        {
                            added880s.Add(seqno, "=880  " + indicators + seqstr + cjkText);
                            li = li.Substring(0, 6) + indicatorsPinyin + "\u001F6880-" + string.Format("{0:00}", seqno) + pinyinText;
                        }
                    }
                    newrec.addLine(li);
                }

                //add the new 880s that have been created
                bool recordChanged = false;
                foreach (KeyValuePair<int, string> keyval in added880s)
                {
                    recordChanged = true;
                    newrec.addLine(keyval.Value, true);

                }
                if (recordChanged)
                {
                    newrecords[entry.Key] = newrec;
                }

                //keep user abreast of progress
                if ((recno % 10 == 0) || (recno == numrecs))
                {
                    this.progressLabel.Text = "Record " + recno + " of " + numrecs + " processed.";
                    this.Refresh();
                }
            }
            return newrecords;
        }

        /*
         * processNumbers: performs additional processing of the pinyin string to handle certain numerical expressions.
         * In the pinyin string, there may be certain characters marked with a "#" suffix.  These include Chinese numerals
         * or other characters that appear in numerical expressions (such as the ordinal marker and time words).  This method
         * determines if these expressions need to be converted to Arabic numerals, does the conversion, and in all cases removes
         * the "#" suffixes.
         */



        private string processNumbers(string pinyinString, string tag)
        {
            string outputString = "";
            bool useNumVersion = false;
            string subfield = "";
            //remove and make a note of subfield delimiter
            if (pinyinString.Substring(0, 1).Equals("\u001F"))
            {
                outputString += pinyinString.Substring(0, 2);
                subfield = pinyinString.Substring(1, 1);
                pinyinString = pinyinString.Substring(2);
                if ((tag.Equals("245") || tag.Equals("830")) && subfield.Equals("n"))
                {
                    useNumVersion = true;
                }
            }

            /*
             * The input string is split, with any space or punctuation character (except for #) as the delimiter.
             * The delimiters will be captured and included in the string of tokens.  Only the even-numbered
             * array elements are the true 'tokens', so the code for processing tokens is run only for even
             * values of j.
             */
            string[] tokens = Regex.Split(pinyinString, @"([^\P{P}#]|\s)");
            string numTokenPattern = @"^([A-Za-z]+)#([0-9]*)$";
            int n = tokens.Count();
            for (int i = 0; i < n; i++)
            {
                string toki = tokens[i];
                if (Regex.IsMatch(toki, numTokenPattern))
                {
                    /*
                     * When a numerical token (containing #) is reached, the inner loop consumes it and all consecutive numerical tokens
                     * found after it.  Two versions of the string are maintained.  The textVersion is the original pinyin (minus the
                     * # suffixes).  In the numVersion, characters representing numbers are converted to Arabic numerals.  When a 
                     * non-numerical token (or end of string) is encountered, the string of numerical tokens is evaluated to determine
                     * which version should be used in the output string.  The outer loop then continues where the inner loop left off.
                     */
                    string textVersion = "";
                    string numVersion = "";
                    for (int j = i; j < n; j++)
                    {
                        string tokj = tokens[j];
                        /* a token without # (or the end of string) is reached */
                        if ((j % 2 == 0 && !Regex.IsMatch(tokj, numTokenPattern)) || j == n - 1)
                        {
                            //If this runs, then we are on the last token and it is numeric. Add text after # (if present) to numerical version
                            if (Regex.IsMatch(tokj, numTokenPattern))
                            {
                                Match m = Regex.Match(tokj, numTokenPattern);
                                textVersion += m.Groups[1].Value;
                                if (m.Groups[2].Value.Equals(""))
                                {
                                    numVersion += m.Groups[1].Value;
                                }
                                else
                                {
                                    numVersion += m.Groups[2].Value;
                                }
                            }
                            //if last token is non-numerical, just tack it on.
                            else if (j == n - 1)
                            {
                                textVersion += tokj;
                                numVersion += tokj;
                            }
                            //if not at end of string yet and token is non-numerical, remove the last delimiter that was appended
                            //(outer loop will pick up at this point)
                            else if (textVersion.Length > 0 && numVersion.Length > 0)
                            {
                                textVersion = textVersion.Substring(0, textVersion.Length - 1);
                                numVersion = numVersion.Substring(0, numVersion.Length - 1);
                            }
                            //evaluate numerical string that has been constructed so far
                            //use num version for ordinals and date strings
                            if (Regex.IsMatch(numVersion, @"^di [0-9]", RegexOptions.IgnoreCase) ||
                                Regex.IsMatch(numVersion, @"[0-9] [0-9] [0-9] [0-9]") ||
                                Regex.IsMatch(numVersion, @"[0-9]+ nian [0-9]+ yue", RegexOptions.IgnoreCase) ||
                                Regex.IsMatch(numVersion, @"[0-9]+ yue [0-9]+ ri", RegexOptions.IgnoreCase) ||
                                useNumVersion
                                )
                            {
                                useNumVersion = true;
                                /*
                                 * At this point, string may contain literal translations of Chinese numerals
                                 * Convert these to Arabic numerals (for example "2 10 7" = "27"). 
                                 */

                                while (Regex.IsMatch(numVersion, @"[0-9] 10+") || Regex.IsMatch(numVersion, @"[1-9]0+ [1-9]"))
                                {
                                    Match m = Regex.Match(numVersion, @"([0-9]+) ([1-9]0+)");
                                    if (m.Success)
                                    {
                                        long sum = Convert.ToInt64(m.Groups[1].Value) * Convert.ToInt64(m.Groups[2].Value);
                                        numVersion = Regex.Replace(numVersion, @"[0-9]+ [1-9]0+", sum.ToString());
                                    }
                                    else
                                    {
                                        Match mb = Regex.Match(numVersion, @"([1-9]0+) ([0-9]+)");
                                        if (mb.Success)
                                        {
                                            long sumb = Convert.ToInt64(mb.Groups[1].Value) + Convert.ToInt64(mb.Groups[2].Value);
                                            numVersion = Regex.Replace(numVersion, @"[1-9]0+ [0-9]+", sumb.ToString());
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }

                                //A few other tweaks
                                numVersion = Regex.Replace(numVersion, "([0-9]) ([0-9]) ([0-9]) ([0-9])", "$1$2$3$4");
                                if ((tag.Equals("245") || tag.Equals("830")) && subfield.Equals("n"))
                                {
                                    while (Regex.IsMatch(numVersion, @"[0-9] [0-9]"))
                                    {
                                        numVersion = Regex.Replace(numVersion, "([0-9]) ([0-9])", "$1$2");
                                    }
                                }
                            }
                            if (useNumVersion)
                            {
                                outputString += numVersion;
                            }
                            else
                            {
                                outputString += textVersion;
                            }
                            //if the end of the string is not reached, backtrack to the delimiter after the last numerical token
                            //(i.e. two tokens ago)
                            if (j < n - 1)
                            {
                                i = j - 2;
                            }
                            else //we are at the end of the string, so we are done!
                            {
                                i = j;
                            }
                            break;
                        }

                        //this is run when we are not yet at the end of the string and have not yet reached a non-numerical token
                        //This is identical to the code that is run above when the last token is numeric.
                        if (j % 2 == 0)
                        {
                            Match m = Regex.Match(tokj, numTokenPattern);
                            textVersion += m.Groups[1].Value;
                            if (m.Groups[2].Value.Equals(""))
                            {
                                numVersion += m.Groups[1].Value;
                            }
                            else
                            {
                                numVersion += m.Groups[2].Value;
                            }
                        }
                        else //a delimiter, just tack it on.
                        {
                            textVersion += tokj;
                            numVersion += tokj;
                        }
                    }
                }
                else // the outer loop has encountered a non-numeric token or delimiter, just tack it on.
                {
                    outputString += toki;
                }
            }
            return outputString;
        }

        /*
         * getPinyin: convert a string representing a MARC field to pinyin
         */

        private string getPinyin(string cjkText, string tag, string indicators)
        {
            string[] subfields = cjkText.Split('\u001F');
            int n = subfields.Count();
            string pinyinString = subfields[0];

            //iterate through subfields
            for (int i = 1; i < n; i++)
            {
                string stri = subfields[i];
                //replace single dash between CJK chars with double
                stri = Regex.Replace(stri, @"(" + cjkPattern + @")[-\uFF0D](" + cjkPattern + @")", "$1--$2");
                string subfield = stri.Substring(0, 1);
                bool isName = false;

                //determine if subfield contains a personal name
                if ((Regex.IsMatch(tag, "[1678]00") && indicators.Substring(0, 1).Equals("1") && subfield.Equals("a")) ||
                    subfield.Equals("r")
                    )
                {
                    isName = true;
                }
                int ni = stri.Length;
                string pyi = '\u001F' + subfield;

                /*
                 * Attempt to find the longest left-anchored substring of the subfield that matches an entry in the Romanization table.
                 * Once it is found, convert to pinyin, and repeat the process on the remaining part of the subfield.
                 * If none is found copy over one character as-is and resume with the next character.
                 */
                bool firstalpha = true;
                for (int j = 1; j < ni; j++)
                {
                    string prevchar = "";
                    if (j > 1)
                    {
                        prevchar = stri.Substring(j - 1, 1);
                    }
                    string nextchar = stri.Substring(j, 1);
                    string pyj = "";
                    if (!Regex.IsMatch(nextchar, @"[A-Za-z0-9 ]")) //don't bother with lookup if next character is alphanumeric
                    {
                        int max = (maxChineseLen < ni - j) ? (int)maxChineseLen : ni - j;
                        /*
                         * This loop tries to find the longest substring that is in the Romanization table.  It starts by 
                         * looking for the string from index j to the end of the subfield (or the maximum length of the 
                         * chinese column in the table, whichever is shorter).  It keeps on removing one character from the 
                         * end until a match is found.
                         */
                        for (int k = max; k > 0; k--)
                        {
                            string sjk = stri.Substring(j, k);
                            if (sjk.Substring(sjk.Length - 1).Equals(" ") || sjk.Equals("'"))
                            {
                                continue;
                            }
                            sjk = sjk.Replace("'", "''");
                            DataRow[] results = pinyinTable.Select("[chinese]='" + sjk + "'");
                            if (results.Count() > 0)
                            {
                                pyj = results[0][1].ToString();
                                //MessageBox.Show(stri + "\n" + sjk + "\n" + pyj);
                                if (!sjk.Equals(pyj))
                                {
                                    if (Regex.IsMatch(pyj,"^[^\\[\\(]") && firstalpha) //capitalize first character of each subfield
                                    {                                        
                                        pyj = pyj.Substring(0, 1).ToUpper() + pyj.Substring(1);
                                        firstalpha = false;
                                    }
                                    j += k - 1;
                                    break;
                                }
                            }                        
                        }
                    } else
                    {
                        firstalpha = false;
                    } 
                    //MessageBox.Show(stri + "\n" + pyi);
                    //various punctutation/spacing tweaks
                    if (pyi.Length > 2 && !pyj.Equals(" "))
                    {
                        string pyprev = pyi.Substring(pyi.Length - 1);
                        string pynext = pyj.Substring(0);
                        //MessageBox.Show("*" + prevchar + "*" + nextchar + "*" + pyprev + "*" + pynext + (Regex.IsMatch(prevchar + nextchar, "(" + cjkPattern + alphanumPattern + ")|(" + alphanumPattern + cjkPattern + ")")));
                        if (Regex.IsMatch(prevchar + nextchar, "(" + cjkPattern + alphanumPattern + ")|(" + alphanumPattern + cjkPattern + ")") ||
                            (Regex.IsMatch(pyprev, @"[,.-]") && Regex.IsMatch(nextchar, cjkPattern)) ||
                            (Regex.IsMatch(pyprev, @"[\]\)]") && Regex.IsMatch(nextchar, cjkPattern)) ||
                            (Regex.IsMatch(prevchar, cjkPattern) && Regex.IsMatch(pynext, @"[\[\(]")) ||
                            (Regex.IsMatch(pyprev, @"[\/:;]") && Regex.IsMatch(nextchar, cjkPattern)) ||
                            (Regex.IsMatch(prevchar, cjkPattern) && Regex.IsMatch(pynext, @"[\/:-]")) ||
                            pyprev.Equals(")") && pynext.Equals("(")
                        )
                        {
                            pyi += " ";
                        }
                    }
                    
                    if (pyj.Length > 0)
                    {
                        pyi += pyj;
                    }
                    else
                    {
                        pyi += nextchar;
                    }
                    
                }
                //capitalization/spacing tweaks
                pyi = Regex.Replace(pyi, @"([;\(]\s*)([a-z])", m => m.Groups[1].Value + m.Groups[2].Value.ToUpper());
                pyi = Regex.Replace(pyi, @"(['\u2018])([^'\u2018\u2019]+)(['\u2019])", m => @" " + m.Groups[1].Value + m.Groups[2].Value.Substring(0, 1).ToUpper() + m.Groups[2].Value.Substring(1) + m.Groups[3].Value + " ");
                pyi = Regex.Replace(pyi, @"([""\u201C])([^""\u201C\u201D]+)([""\u201D])", m => @" " + m.Groups[1].Value + m.Groups[2].Value.Substring(0, 1).ToUpper() + m.Groups[2].Value.Substring(1) + m.Groups[3].Value + " ");

                pyi = processNumbers(pyi, tag);
;
                if (isName) //special formatting for personal names
                {
                    string possibleComma = ",";
                    if (subfield.Equals("r"))
                    {
                        possibleComma = "";
                    }

                    string possibleApos = "";
                    Match m = Regex.Match(pyi, @"((?:\u001F[a-z]\([^\)]*\) ?)?)([^\s,]+),?\s+(\S+)\s*(.*)$");
                    if (m.Success)
                    {
                        if (m.Groups[4].Value.Length > 0 && Regex.IsMatch(m.Groups[4].Value.Substring(0, 1), "[aeiou]"))
                        {
                            possibleApos = "'";
                        }
                        pyi = m.Groups[1].Value +
                           m.Groups[2].Value.Substring(0, 1).ToUpper() + m.Groups[2].Value.Substring(1) + possibleComma + " " +
                           m.Groups[3].Value.Substring(0, 1).ToUpper() + m.Groups[3].Value.Substring(1) + possibleApos +
                           m.Groups[4].Value;
                    }
                }
                pinyinString += pyi;
            }
            //more spacing/capitalization tweaks
            pinyinString = Regex.Replace(pinyinString, @"\s\s+", " ");
            pinyinString = Regex.Replace(pinyinString, @"(\u001F\S)\s+", "$1");
            //pinyinString = Regex.Replace(pinyinString, @"^(\u001F.\[)([a-z])", m => m.Groups[1].Value + m.Groups[2].Value.ToUpper());
            return pinyinString;
        }

        /*
         * refreshTagLists(): refresh the main dialog, showing which tags have been chosen for conversion and which have not.
         */

        private void refreshTagLists()
        {
            availableTagsList.DataSource = null;
            availableTagsList.DataSource = availableTags.ToList<string>();
            selectedTagsList.DataSource = null;
            selectedTagsList.DataSource = selectedTags.ToList<string>();
        }

        /*
         * Enables/disables add button based on whether the available tags column is empty
         */

        private void availableTagsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (availableTagsList.SelectedIndex > -1)
            {
                addButton.Enabled = true;
            }
            else
            {
                addButton.Enabled = false;
            }
        }

        /*
         * Enables/disables remove button based on whether the selected tags column is empty
         */

        private void selectedTagsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedTagsList.SelectedIndex > -1)
            {
                removeButton.Enabled = true;
            }
            else
            {
                removeButton.Enabled = false;
            }
        }

        /*
         * move tag from selected to available column
         */

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (selectedTagsList.SelectedIndex > -1)
            {
                string item = selectedTagsList.SelectedItem.ToString();
                availableTags.Add(item);
                selectedTags.Remove(item);
                refreshTagLists();
                if(!swapCheckbox.Checked)
                {
                    field880radio.Enabled = false;
                    fieldOriginalRadio.Enabled = false;
                }
            }
        }

        /*
         * move tag from available to selected column
         */

        private void addButton_Click(object sender, EventArgs e)
        {
            if (availableTagsList.SelectedIndex > -1)
            {
                string item = availableTagsList.SelectedItem.ToString();
                selectedTags.Add(item);
                availableTags.Remove(item);
                refreshTagLists();
            }
            field880radio.Enabled = true;
            fieldOriginalRadio.Enabled = true;
        }

        /*
         * cancel
         */

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         * add the pinyin, reconstruct file, convert records back to MARC-8 if needed, save
         */

        private void convertButton_Click(object sender, EventArgs e)
        {
            SortedDictionary<int, ChineseRecord> convertedRecords = addPinyinToRecords();
            int modRecCount = convertedRecords.Count;            
            this.progressLabel.Text = "Saving file...";
            this.Refresh();
            foreach (KeyValuePair<int, ChineseRecord> keyval in convertedRecords)
            {
                int index = keyval.Key;
                allRecords[index] = new ChineseRecord(convertedRecords[index].toString());
            }
            if (swapCheckbox.Checked)
            {
                modRecCount = swapParallelFields(field880radio.Checked);
            }
            writeRecordsToFile();

            string msg = "";
             
            if (convertedRecords.Count > 0)
            {
                msg = "Romanization was added to " + convertedRecords.Count + " records.\n";
            }
            if(msg == "" || (swapCheckbox.Checked && modRecCount > convertedRecords.Count))
            {
                msg += modRecCount + " records were modified.";
            }
            MessageBox.Show(msg);
            this.Close();
        }

        private void writeRecordsToFile()
        {
            variables.objEditor.SetText = "";
            string filename = variables.objEditor.Loaded_File;
            variables.objEditor.SaveFile(filename);
            StreamWriter writer = variables.objEditor.WriteFile(filename);

            //go through all records.  If a record has not had pinyin added, copy back into file as-is
            //otherwise, copy over pinyin version, converting back to MARC-8 if needed
            int count = 0;
            foreach (KeyValuePair<int, ChineseRecord> keyval in allRecords)
            {
                int index = keyval.Key;
                string recStr = keyval.Value.toString();
                recStr = recStr.Replace('\u001F', '$');
                count++;
                variables.objEditor.WriteLine(writer, recStr + Environment.NewLine);
            }
            variables.objEditor.CloseFile(writer);
            variables.objEditor.LoadFile(filename);
        }

        private void swapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTagsList.Items.Count == 0)
            {
                if (swapCheckbox.Checked)
                {
                    field880radio.Enabled = true;
                    fieldOriginalRadio.Enabled = true;
                }
                else
                {
                    field880radio.Enabled = false;
                    fieldOriginalRadio.Enabled = false;
                }
            }
        }
    }
}
