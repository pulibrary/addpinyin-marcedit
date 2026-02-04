# AddPinyin Plugin for MarcEdit
A plugin for MarcEdit that allows the user to convert Chinese text in MARC records to Hanyu Pinyin, with certain catalog-record-specific formatting applied. 

## Installation/Usage

1. [Download the installer](https://github.com/pulibrary/addpinyin-marcedit/releases/latest/download/InstallAddPinyin.exe)
  - Compatible with the Windows version of MarcEdit (between versions 6 and 7.3.x - not yet compatiable with 7.5.x).
  - This plugin does not need to be installed as Administrator.  It should be installed while logged in as the user that will be using the software.
2. Create a backup of the MARC record file to be converted.
3. Convert the file to MRK format (using the MarcBreaker tool in MarcEdit) and open in MarcEditor. The file must be encoded in UTF-8.  For MARC-8 files, use MarcBreaker to convert the file to UTF-8 before running the plugin, then convert it back to MARC-8 afterwards, if needed.
4. Open the "Plugins" menu and select "AddPinyin". A dialog will appear warning you that the conversion cannot be undone, and that the MRK file will be automatically saved after conversion. (This is why it is important to back the file up). Click "OK".
5. A dialog will appear asking you which fields to generate romanization for. Certain fields are selected by default. Use the arrow buttons to specify which fields you would like to convert.
6. Select whether you want the pinyin to be placed in the original field or the corresponding 880, and also whether you want to swap the order of existing parallel fields accordingly.
7. Click the “Convert” button.
8. After romanization is complete, the updated records will be displayed in the MarcEditor. Compile file back to MRC format by opening the “File” menu and selecting “Compile File into MARC”.

## Functionality
For each converted field, an 880 field is created containing either the original or romanized text (as specified by the user). A subfield 6 with the appropriate linkage value is automatically added to both the original field and the 880. 

The plugin's functionality complements that of the [OCLC Connexion Pinyin Conversion Macro](https://github.com/pulibrary/oclcpinyin); whereas the OCLC Macro is run on individual fields within a single record, the MarcEdit Plugin is designed for batch processing.  Also, since the plugin runs within MarcEdit, it is independent of a specific catalog.

This plugin can be run on files containing both Chinese and non-Chinese records. The plugin examines the 008 field to identify Chinese records, and leaves everything else untouched.  If a record has romanization added for some but not all Chinese fields, the plugin can add romanization for the remaining unconverted fields (if desired).  The main dialog of the program will tell you how many records contain Chinese text that could potentially be converted.  The user also has the option to swap the order of parallel fields already existing in the records.  (Swapping the fields can be done even if no other conversion is performed on the record set).

The plugin generates pinyin using ALA-LC standards. The output is similar to that produced by the OCLC Macro. Please see the [documentation](https://github.com/pulibrary/oclcpinyin#functionality) for this marco for specifics. It is difficult to automate romanization with 100% accuracy, so it is always beneficial to manually proofread the results when practical. However, most of the needed adjustments will have to do with spacing, capitalization, and punctuation, not the pinyin itself. Efforts have been made to keep even these minor inaccuracies to a minimum. 

## Feedback
If you notice any errors or would like to suggest new phrases to be included in the dictionary, please go to the "Issues" tab at the top of this github page, and click the "New Issue" button. 

## Sources
The macro contains a dictionary of Chinese characters and phrases based on three sources:
- The [Unihan database](http://unicode.org/charts/unihan.html), copyright 1991-2024, Unicode, Inc. Last updated 2023-07-15.
- [CC-CEDICT](http://www.mdbg.net/chinese/dictionary?page=cedict), copyright 2024, MDBG. Last updated 2024-07-10.
- User feedback.
