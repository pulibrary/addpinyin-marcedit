using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * ChineseRecord is a class representing a MARC record that may contain Chinese text.
 */

namespace AddPinyin
{
    public class ChineseRecord
    {
        private List<string> lines;
        private string[] separators =  {Environment.NewLine};
        public ChineseRecord(string rec = "")
        {
            string[] reclines = rec.Split(separators,StringSplitOptions.RemoveEmptyEntries);
            lines = new List<string>(reclines);
        }
        /* Adds a line (field) to the current record.
         * If collate is set to 'false', new line is added to the end of the record.
         * Otherwise, an attempt is made to insert the field "in order", though it is not assumed that the rest of the fields
         * are in order.  So, for example, the new line would be inserted before the first field that has a higher tag number,
         * even if lower tag numbers exist further down in the record.  For lines with the same tag number, the rest of the field
         * is taken into account.  In particular, multiple 880 fields are sorted according to the occurrence numbers in subfield 6.
         */
        public void addLine(string line, bool collate = false)
        {
            //MessageBox.Show(line);
            if (collate)
            {
                int n = lines.Count;
                int insertPoint = -1;

                /* 
                 * The new field is compared to the existing fields of the record, starting from the beginning. 
                 * It is inserted before the first field that is greater than it according to a string comparison.
                 * The strings to be compared are 'scrubbed', i.e. indicators are removed.  If the data part of the field
                 * begins with subfield 6, the part of the linkage string up to the occurrence number is stripped. This
                 * is so that multiple 880s are sorted by occurrence numbers.
                 */

                //scrub new line
                string lineScrubbed = line.Substring(0, 4) + line.Substring(8);
                if (lineScrubbed.Substring(4, 2).Equals("\u001F6"))
                {
                    lineScrubbed = lineScrubbed.Substring(0, 4) + lineScrubbed.Substring(10);
                }
                //LDR field always goes at the beginning
                if (line.Substring(0, 4).Equals("=LDR"))
                {
                    insertPoint = 0;
                }
                else
                {
                    //go through other lines in the record
                    for (int i = 0; i < n; i++)
                    {
                        string li = lines[i];
                        //skip control fields (it is assumed these are being inserted in the desired order)
                        if (Regex.IsMatch(li.Substring(1, 1),@"[A-Z]") || li.Substring(1,2).Equals("00"))
                        {
                            continue;
                        }
                        
                        //scrub line i
                        li = li.Substring(0, 4) + li.Substring(8);
                        if (li.Substring(4, 2).Equals("\u001F6"))
                        {
                            li = li.Substring(0, 4) + li.Substring(10);
                        }
                        //MessageBox.Show(li + "\n" + lineScrubbed);
                        if (lineScrubbed.CompareTo(li) < 0)
                        {
                            insertPoint = i;
                            break;
                        }
                    }
                }
                if (insertPoint > -1)
                {
                    lines.Insert(insertPoint, line);
                }
                else
                {
                    lines.Add(line);
                }
            }
            else
            {
                lines.Add(line);
            }
        }
        public List<string> getLines()
        {
            return lines;
        }
        public string toString()
        {
            return String.Join(Environment.NewLine, lines);
        }
    }
}
