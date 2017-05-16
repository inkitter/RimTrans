using System;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.IO;
using System.Xml;


namespace RimTrans
{
    static class StaticVars
    {
        public const string DIRBASE = "Mods\\";
        public const string DIRDEFS = "Defs\\";
        public const string DIRLANGUAGES = "Languages\\";

        public const string UserDictCSV = "ymldict.csv";
        public const string DirOldBase = "old\\";
        public const string DIRCN = "chn\\";
        public const string DIRCNen = "chn\\english\\";
        public const string DIRCNcn = "chn\\simp_chinese\\";
    }
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmTranslator());
        }
    }
    public class NodeInfo
    {
        public string TypeFolder { get; set; }
        public string SubFolder { get; set; }
        public string FileName { get; set; }
        public string NodeName { get; set; }
        public string NodeText { get; set; }
        public string FilePath
        {
            get
            {
                if (SubFolder == "") { return TypeFolder +"\\" + FileName; }
                return TypeFolder + "\\" + SubFolder + "\\" + FileName;
            }
        }
        public string NodeKey
        {
            get
            {
                return (FilePath +"."+ NodeName).ToLower();
            }
        }
    }
    public class XMLdata
    {
        private bool isedited;
        private string contentdest,contenteng;  //翻译后内容
        private NodeInfo nodeinfo;

        private XMLdata()
        {
            isedited = false;
            contenteng = "";
            contentdest = "";
            nodeinfo = new NodeInfo();
        }
        public XMLdata(NodeInfo Info):this()
        {
            nodeinfo = NewNodeinfo(Info);
        }
        public XMLdata(NodeInfo Info,bool IsEnglishorDest) : this()
        {
            if (IsEnglishorDest == true)
            {
                nodeinfo = NewNodeinfo(Info.FileName, Info.NodeName, "", Info.SubFolder, Info.TypeFolder);
                contenteng = Info.NodeText;
            }
            else
            {
                nodeinfo = NewNodeinfo(Info.FileName, Info.NodeName, "", Info.SubFolder, Info.TypeFolder);
                contentdest = Info.NodeText;
            }
        }

        private NodeInfo NewNodeinfo(string filename,string nodename,string nodetext,string subfolder,string typefolder)
        {
            return new NodeInfo() {FileName=filename,NodeName=nodename,NodeText=nodetext,SubFolder=subfolder,TypeFolder=typefolder };
        }
        private NodeInfo NewNodeinfo(NodeInfo Info)
        {
            return new NodeInfo() { FileName = Info.FileName, NodeName = Info.NodeName, NodeText = Info.NodeText, SubFolder = Info.SubFolder, TypeFolder = Info.TypeFolder };
        }
        public string LabelName
        {
            get
            {
                return nodeinfo.NodeName;
            }
        }
        public string ContentEng
        {
            get
            {
                if (contenteng == "") { return nodeinfo.NodeText; }
                return contenteng;
            }
        }

        public string ContentDest
        {
            get
            {
                if (contentdest == "") { return ContentEng; }
                return contentdest;
            }
        }

        

        public string TypeFolder
        {
            get
            {
                return nodeinfo.TypeFolder;
            }
        }
        public string SubFolder
        {
            get
            {
                return nodeinfo.SubFolder;
            }
        }
        public string FileName
        {
            get
            {
                return nodeinfo.FileName;
            }
        }
        
        public string FilePath
        {
            get
            {
                return nodeinfo.FilePath;
            }
        }
        public string NodeKey()
        {
            return nodeinfo.NodeKey.ToLower();
        }


        public void ApplyLine(string ApplyText)
        {
            contentdest = ApplyText;
            isedited = true;
        }

        public void SetDest(string ApplyText)
        {
            contentdest = ApplyText;
        }

        public void SetEng(string EngText)
        {
            contenteng = EngText;
        }

        public bool IsEdited
        {
            get
            {
                return isedited;
            }
        }

        public bool SameInToAndFrom()
        {
            if (contenteng != "" && contenteng != contentdest)
            {
                return false;
            }
            if (nodeinfo.NodeText != contentdest) { return false; }
            return true;
        }
    }

    public class TranslationResultVIP
    {
        //错误码，翻译结果无法正常返回
        public string Error_code { get; set; }
        public string Error_msg { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Query { get; set; }
        //翻译正确，返回的结果
        //这里是数组的原因是百度翻译支持多个单词或多段文本的翻译，在发送的字段q中用换行符（\n）分隔
        public Translation[] Trans_result { get; set; }
    }

    public class TranslationResult
    {
        //public string From { get; set; }
        //public string To { get; set; }
        public Translation[] Data { get; set; }
    }

    public class Translation
    {
        public string Src { get; set; }
        public string Dst { get; set; }
    }

    public class FileExistInfo
    {
        public bool IsExist { get; set; }
        public string FileName { get; set; }
    }

    public class LoadedFileInfo
    {
        public bool IsTranslationExist { get; set; }
        public string FileName { get; set; }
    }

    public static class XMLTools
    {
        private static TranslationResult GetTranslationFromBaiduFanyi(string q)
        {
            WebClient wc = new WebClient();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            TranslationResult result = jss.Deserialize<TranslationResult>(wc.DownloadString("http://fanyi.baidu.com/transapi?from=en&to=zh&query=" + WebUtility.UrlEncode(q)));
            return result;
            //解析json
        }

        public static string GetTranslatedTextFromAPI(string TexttoTranslate)
        {
            if (TexttoTranslate != "")
            {
                TranslationResult result = GetTranslationFromBaiduFanyi(TexttoTranslate);
                return result.Data[0].Dst;
            }
            return "Nothing";
        }
        // 用于从baidu 翻译API获取翻译。

        public static string RegexGetWith(string RegText, string RegexRule)
        {
            Regex Reggetname = new Regex(RegexRule, RegexOptions.None);
            StringBuilder returnString = new StringBuilder();
            var matches = Reggetname.Matches(RegText);

            foreach (var item in matches)
            {
                returnString.Append(item.ToString());
            }
            return returnString.ToString();
        }
        public static string RegexGetName(string RegText)
        {
            return RegexGetWith(RegText, "(^.*?):.*?(?=\")");
        }
        public static string RegexGetValue(string RegText)
        {
            return RegexGetWith(RegText, "(?<=(\\s\")).+(?=\")");
        }
        public static string RegexGetNameOnly(string RegText)
        {
            RegText = RegText.Replace(" ", "");
            return RegexGetWith(RegText, "^.*(?=:)");
        }
        public static string RegexRemoveColorSign(string RegText)
        {
            return RegexGetWith(RegText, "(?<=(§.)).+(?=(§!))");
        }
        private static string RegexStringWordBoundry(string input)
        {
            return @"(\W|^)" + input + @"(\W|$)";
        }
        public static bool RegexContainsWord(string input, string WordToMatch)
        {
            if (Regex.IsMatch(input, RegexStringWordBoundry(WordToMatch), RegexOptions.IgnoreCase)) { return true; }
            return false;
        }
        // 用于截取

        public static void OpenWithBrowser(string TextToTranslate,string APIEngine)
        {
            StringBuilder StrOpeninBrowser = new StringBuilder();
            switch (APIEngine)
            {
                case "Google":
                    StrOpeninBrowser.Append("http://translate.google.com/?#auto/zh-CN/");
                    break;
                case "Baidu":
                    StrOpeninBrowser.Append("http://fanyi.baidu.com/?#en/zh/");         
                    break;
                case "Help":
                    StrOpeninBrowser.Append("https://github.com/inkitter/pdx-ymltranslator");
                    break;
                default:
                    StrOpeninBrowser.Append("http://fanyi.baidu.com/?#en/zh/");
                    break;
            }
            StrOpeninBrowser.Append(TextToTranslate);
            System.Diagnostics.Process.Start(StrOpeninBrowser.ToString());
        }
        // 用于默认浏览器打开翻译网页

        public static string RemoveReturnMark(string input)
        {
            StringBuilder RemoveReturnText = new StringBuilder();
            RemoveReturnText.Append(input);
            RemoveReturnText.Replace("\r", "");
            RemoveReturnText.Replace("\n", "");
            return RemoveReturnText.ToString();
        }
        // 用于移除换行符。

        public static string RemoveSpace(string input)
        {
            return input.Replace(" ", "");
        }

        public static string ReplaceWithUserDict(string input, Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                Regex rgx = new Regex(RegexStringWordBoundry(kvp.Key), RegexOptions.IgnoreCase);
                input = rgx.Replace(input, " "+kvp.Key + "<" + kvp.Value + "> ");
            }
            return input;
        }

        public static Dictionary<string, string> BuildDictionary(List<string> list)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (string line in list)
            {
                string vn = RegexGetNameOnly(RegexGetName(line));
                if (!returnDict.ContainsKey(vn))
                {
                    returnDict.Add(vn, line);
                }
            }
            return returnDict;
        }

        public static string ToSimplifiedChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.SimplifiedChinese, 0);
        }
        public static string ToTraditionalChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.TraditionalChinese, 0);
        }

        public static List<XMLdata> ReadFolder(string modname,string language)
        {
            string OriPath = StaticVars.DIRBASE + modname + "\\Defs\\";
            string EngPath = StaticVars.DIRBASE + modname + "\\Languages\\English\\";
            string ChnPath = StaticVars.DIRBASE + modname + "\\Languages\\" + language + "\\";
            List<NodeInfo> nodeori = new List<NodeInfo>();
            List<NodeInfo> nodeeng = new List<NodeInfo>();
            List<NodeInfo> nodechn = new List<NodeInfo>();
            XmlDocument xdoc = new XmlDocument() ;
            XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true };

            NodeInfo XNodeInfo = new NodeInfo() {FileName="",SubFolder="",TypeFolder="",NodeName="",NodeText="" };
            XMLdata Xdata;
            List<XMLdata> lstXMLdata= new List<XMLdata>();
            Dictionary<string, NodeInfo> DestDict = new Dictionary<string, NodeInfo>();
            Dictionary<string, NodeInfo> EngDict = new Dictionary<string, NodeInfo>();

            XNodeInfo.TypeFolder = "Keyed";
            if (Directory.Exists(EngPath + "Keyed\\"))
            {
                nodeeng.AddRange(ReadFileList(Directory.GetFiles(EngPath + "Keyed\\")));
            }
            if (Directory.Exists(ChnPath + "Keyed\\"))
            {
                nodechn.AddRange(ReadFileList(Directory.GetFiles(ChnPath + "Keyed\\")));
            }
            // 读取Keyed下的中文和英文文本

            XNodeInfo.TypeFolder = "DefInjected";
            if (Directory.Exists(EngPath + "\\DefInjected\\"))
            {
                foreach (string dir in Directory.GetDirectories(EngPath + "\\DefInjected\\"))
                {
                    XNodeInfo.SubFolder = Path.GetFileName(dir);
                    nodeeng.AddRange(ReadFileList(Directory.GetFiles(Path.GetFullPath(dir))));
                }
            }
            if (Directory.Exists(ChnPath + "\\DefInjected\\"))
            {
                foreach (string dir in Directory.GetDirectories(ChnPath + "\\DefInjected\\"))
                {
                    XNodeInfo.SubFolder = Path.GetFileName(dir);
                    nodechn.AddRange(ReadFileList(Directory.GetFiles(Path.GetFullPath(dir))));
                }
            }
            // 读取DefInjected下的中文和英文文本

            foreach (NodeInfo no in nodechn)
            {
                DestDict.Add(no.NodeKey, new NodeInfo(){ FileName = no.FileName, NodeName = no.NodeName, NodeText = no.NodeText, SubFolder = no.SubFolder, TypeFolder = no.TypeFolder });
            }
            // 把中文内容生成词典

            foreach (NodeInfo no in nodeeng)
            {
                EngDict.Add(no.NodeKey, new NodeInfo() { FileName = no.FileName, NodeName = no.NodeName, NodeText = no.NodeText, SubFolder = no.SubFolder, TypeFolder = no.TypeFolder });
            }
            // 将英文文本生成词典

            ////////////////////

            // 读取Def文件夹中有关英文文本的定义部分
            nodeori = ReadOriginal(OriPath);
            foreach (NodeInfo ss in nodeori)
            {
                Xdata = new XMLdata(ss);
                lstXMLdata.Add(Xdata);
            }
            // 将定义的英文文本生成初始总表

            foreach (XMLdata data in lstXMLdata)
            {
                EngDict.TryGetValue(data.NodeKey(), out NodeInfo s);
                if (s != null)
                {
                    string t = s.NodeText;
                    data.SetDest(t);
                }
                EngDict.Remove(data.NodeKey());
            }
            // 从英文词典中查询词条并插入到总表中。

            foreach (NodeInfo ss in EngDict.Values)
            {
                Xdata = new XMLdata(ss, true);
                lstXMLdata.Add(Xdata);
            }
            // 英文字典剩余的部分追加到总表。用以保留自己额外添加的文本
            ////////////////////

            foreach (XMLdata data in lstXMLdata)
            {
                DestDict.TryGetValue(data.NodeKey(), out NodeInfo s);
                if (s!=null)
                {
                    string t = s.NodeText;
                    data.SetDest(t);
                }
                
                DestDict.Remove(data.NodeKey());
            }
            // 从中文字典中查询词条并插入到总表中。

            foreach (NodeInfo ss in DestDict.Values)
            {
                Xdata = new XMLdata(ss, false);
                lstXMLdata.Add(Xdata);
            }
            // 中文字典剩余的部分追加到总表。用以保留自己额外添加的文本

            return lstXMLdata;

            ////////////////////////////////// 以下是调用到的子函数

            List<NodeInfo> ReadFileList(string[] list)
            {
                List<NodeInfo> data = new List<NodeInfo>();
                if (list.Length > 0)
                {
                    foreach (string str in list)
                    {
                        XNodeInfo.FileName = Path.GetFileName(str);
                        xdoc.Load(XmlReader.Create(str, settings));

                        XmlNode xn = xdoc.ChildNodes[1];
                        foreach (XmlNode nn in xn.ChildNodes)
                        {
                            data.Add(new NodeInfo() { FileName=XNodeInfo.FileName,NodeName= nn.Name, NodeText= nn.InnerText,SubFolder=XNodeInfo.SubFolder,TypeFolder=XNodeInfo.TypeFolder });
                        }
                    }
                }
                return data;
            }

            List<NodeInfo> ReadOriginal(string path)
            {
                List<NodeInfo> data = new List<NodeInfo>();
                NodeInfo nodels = new NodeInfo();
                string dename="";
                if (!Directory.Exists(path)) { return data; }

                foreach (string folders in Directory.GetDirectories(path))
                {
                    foreach (string file in Directory.GetFiles(folders))
                    {
                        nodels = new NodeInfo() { TypeFolder = "DefInjected" };

                        nodels.FileName = Path.GetFileName(file);
                        xdoc.Load(XmlReader.Create(file, settings));
                        XmlNode n = xdoc.ChildNodes[1];
                        XmlNodeList lst = n.ChildNodes;
                        foreach (XmlNode xn in lst)
                        {
                            nodels.SubFolder = xn.Name;
                            foreach (XmlNode xnn in xn.ChildNodes)
                            {
                                if (xnn.Name== "defName" ) { dename = xnn.InnerText; }
                                switch (xnn.Name.ToLower())
                                {
                                    case "defname":
                                        dename = xnn.InnerText;
                                        break;
                                    default:
                                        break;
                                }
                                if (dename != "")
                                {
                                    switch (xnn.Name.ToLower())
                                    {
                                        case "label":
                                        case "description":
                                        case "deathmessage":
                                        case "jobstring":
                                            
                                            nodels.NodeName = GetNodeName(dename, xnn.Name.ToLower());
                                            nodels.NodeText = xnn.InnerText;
                                            data.Add(Getnewnode(nodels));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            dename = "";
                        }
                    }
                }
                return data;
            }
        }
        private static NodeInfo Getnewnode(NodeInfo input)
        {
            return new NodeInfo() {FileName=input.FileName,NodeName=input.NodeName,NodeText=input.NodeText,SubFolder=input.SubFolder,TypeFolder=input.TypeFolder };
        }
        private static string GetNodeName(string defName,string typeName)
        {
            return defName+"."+typeName;
        }
        public static void SaveMod(string modname, List<XMLdata> indata, string language)
        {
            Dictionary<string, List<XMLdata>> dictfiles = new Dictionary<string, List<XMLdata>>();

            string ChnPath = StaticVars.DIRBASE + modname + "\\Languages\\" + language + "\\newTransed\\";
            if (!Directory.Exists(ChnPath)) { Directory.CreateDirectory(ChnPath); }

            foreach (XMLdata indatasingle in indata)
            {
                List<XMLdata> newlist;
                if (dictfiles.ContainsKey(indatasingle.FilePath))
                {
                    dictfiles.TryGetValue(indatasingle.FilePath, out List<XMLdata> list);
                    list.Add(indatasingle);
                    dictfiles.Remove(indatasingle.FilePath);
                    dictfiles.Add(indatasingle.FilePath, list);
                }
                else
                {
                    newlist = new List<XMLdata>();
                    newlist.Add(indatasingle);
                    dictfiles.Add(indatasingle.FilePath, newlist);
                }
            }
            foreach (List<XMLdata> lst in dictfiles.Values)
            {
                string path = ChnPath + lst[0].FilePath;

                if (!Directory.Exists(Path.GetDirectoryName(path))) { Directory.CreateDirectory(Path.GetDirectoryName(path)); }

                XmlTextWriter xml = new XmlTextWriter(path, Encoding.UTF8);

                xml.Formatting = Formatting.Indented;
                xml.WriteStartDocument();
                xml.WriteStartElement("LanguageData");

                foreach (XMLdata dat in lst)
                {
                    xml.WriteStartElement(dat.LabelName);
                    xml.WriteString(dat.ContentDest);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }

        }
    }

}
