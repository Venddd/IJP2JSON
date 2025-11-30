using HtmlAgilityPack;
using System.Text.Json;
using System.Collections;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static ijp2json.ConsoleApp;
namespace ijp2json
{
    public static class Variables
    {
        public static int ID;
    }

    public class wordData
    {
        public int? ID { get; set; }
        public string? druh { get; set; }
        public string? slovo { get; set; }
        public string? deleni { get; set; }
        public string? rod { get; set; }

        public string[] firstPadSingular { get; set; }
        public string[] firstPadPlural { get; set; }
        public string[] secondPadSingular { get; set; }
        public string[] secondPadPlural { get; set; }
        public string[] thirdPadSingular { get; set; }
        public string[] thirdPadPlural { get; set; }
        public string[] fourthPadSingular { get; set; }
        public string[] fourthPadPlural { get; set; }
        public string[] fifthPadSingular { get; set; }
        public string[] fifthPadPlural { get; set; }
        public string[] sixthPadSingular { get; set; }
        public string[] sixthPadPlural { get; set; }
        public string[] seventhPadSingular { get; set; }
        public string[] seventhPadPlural { get; set; }

        
    }
    class ConsoleApp
    {
        static void Informer(int ColorId, string text, bool HoldThenExit)
        {
            ConsoleColor ColorBkg = ConsoleColor.Black;

            switch (ColorId)
            {
                case -1:
                    ColorBkg = ConsoleColor.Black;
                    break;
                case 0:
                    ColorBkg = ConsoleColor.DarkBlue;
                    break;
                case 1:
                    ColorBkg = ConsoleColor.DarkCyan;
                    break;
                case 2:
                    ColorBkg = ConsoleColor.DarkRed;
                    break;
                case 3:
                    ColorBkg = ConsoleColor.Green;
                    break;
            }
            Console.BackgroundColor = ColorBkg;

            Console.WriteLine(text);

            Console.BackgroundColor = ConsoleColor.Black;

            if (HoldThenExit == true)
            {
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
        }
        static void Main()
        {
            Informer(1, "by vend.", false);

            Informer(0, "[i] Select needed script", false);
            Informer(-1, " ", false);
            Informer(0, "1 - Ijp", false);
            Informer(0, "2 - GenSku", false);
            int userChoice = Convert.ToInt32(Console.ReadLine());

            switch (userChoice)
            {
                case 1:
                    Ijp2Json();
                    break;
                case 2:
                    GenSku();
                    break;
            }
        }

        static void Ijp2Json()
        {
            Ijp2JsonMenu();
            static void Ijp2JsonMenu()
            {
                Informer(1, "Ijp2Json", false);

                Informer(0, "[i] Select needed function", false);
                Informer(-1, " ", false);
                Informer(0, "1 - Get 'Slovo' from Ijp", false);
                Informer(0, "2 - Get rid of duplicates in input list", false);
                int userChoice = Convert.ToInt32(Console.ReadLine());

                switch (userChoice)
                {
                    case 1:
                        IjpMenu();
                        break;
                    case 2:
                        getRidFromDublicates();
                        break;
                }
            }

            static void getRidFromDublicates()
            {
                string inputList;
                using (var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"input\input.txt", Encoding.UTF8))
                {
                    inputList = streamReader.ReadToEnd();
                }

                string inputListNOwhitespaces = inputList.Replace(" ", "");
                string[] inputListNOdublicates = inputListNOwhitespaces.Split(',').Distinct().ToArray();

                foreach (string slovo in inputListNOdublicates)
                {
                    using (FileStream iListNoDublicates = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"input\inputNoDublicates.txt", FileMode.Append))
                    {
                        byte[] InBinary = System.Text.Encoding.Default.GetBytes(slovo + ", ");
                        iListNoDublicates.Write(InBinary);
                    }
                }
                Informer(3, "[i] Finished!", true);
            }
            static void IjpMenu()
            {
                Informer(2, "[!] Write the last word ID!", false);
                int lastID = Convert.ToInt32(Console.ReadLine());
                Variables.ID = lastID;
                getSlovolist();
            }
            static void getSlovolist()
            {
                string list;
                using (var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"input\input.txt", Encoding.UTF8))
                {
                    list = streamReader.ReadToEnd();
                }

                string listNOwhitespaces = list.Replace(" ", "");
                string[] slovoarray = listNOwhitespaces.Split(',').Distinct().ToArray();

                foreach (string slovo in slovoarray)
                {
                    Informer(0, "[i] Now working on " + slovo + "...", false);
                    checkErrorId("?id=", true, slovo);
                    //Console.ReadKey();
                }
            }

            static void checkErrorId(string searchAttribute, bool pasteSlovo, string slovo)
            {
                string url;
                if (pasteSlovo == true)
                {
                    url = "https://prirucka.ujc.cas.cz/" + searchAttribute + slovo;
                }
                else
                {
                    url = "https://prirucka.ujc.cas.cz/" + searchAttribute;
                }

                var httpClient = new HttpClient();
                var html = httpClient.GetStringAsync(url).Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                //ERROR_check
                var content = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='content']");
                if (content.InnerHtml.Trim().StartsWith("<p><font color="))
                {
                    Console.WriteLine("ERRORWITH ?ID");
                    checkDoubleSearch("?slovo=", true, slovo);
                }
                else
                {
                    getSlovo("?id=", true, slovo, true, htmlDocument);
                }
            }

            static void checkDoubleSearch(string searchAttribute, bool pasteSlovo, string slovo)
            {
                HtmlAgilityPack.HtmlDocument placeholder = null;

                string url;
                if (pasteSlovo == true)
                {
                    url = "https://prirucka.ujc.cas.cz/" + searchAttribute + slovo;
                }
                else
                {
                    url = "https://prirucka.ujc.cas.cz/" + searchAttribute;
                }

                var httpClient = new HttpClient();
                var html = httpClient.GetStringAsync(url).Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                //doubleSearchResult_check

                if (htmlDocument.DocumentNode.SelectSingleNode("//div[@class='screen']") != null)
                {
                    // Console.WriteLine(slovo);
                    //  Console.WriteLine(slovo.Length);
                    var correctLink = htmlDocument.DocumentNode.SelectSingleNode("//td[@style]");
                    string htmlNodeLink = correctLink.FirstChild.InnerHtml.Trim();
                    //  Console.WriteLine(htmlNodeLink);
                    //  Console.WriteLine(htmlNodeLink.Length);
                    string link = htmlNodeLink.Substring(37, htmlNodeLink.Length - 37 - 6 - slovo.Length);
                    // Console.WriteLine(link);
                    //Console.ReadKey();


                    getSlovo(link, false, slovo, false, placeholder);
                }

                else
                {
                    getSlovo("?slovo=", true, slovo, true, htmlDocument);
                }
            }

            static void getSlovo(string searchAttribute, bool pasteSlovo, string slovo, bool getHtml, HtmlAgilityPack.HtmlDocument passedHtml)
            {
                HtmlAgilityPack.HtmlDocument htmlDocument;

                if (getHtml == true)
                {
                    htmlDocument = passedHtml;
                }
                else
                {
                    string url;
                    if (pasteSlovo == true)
                    {
                        url = "https://prirucka.ujc.cas.cz/" + searchAttribute + slovo;
                    }
                    else
                    {
                        url = "https://prirucka.ujc.cas.cz/" + searchAttribute;
                    }

                    var httpClient = new HttpClient();
                    var html = httpClient.GetStringAsync(url).Result;
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                }

                getSubstantiva(htmlDocument);

                static void getSubstantiva(HtmlAgilityPack.HtmlDocument htmlDocument)
                {


                    // Word
                    var hlavickaElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='hlavicka']");
                    var wordElement = htmlDocument.DocumentNode.SelectSingleNode("//h2[@class='ks']").FirstChild;
                    Console.WriteLine(wordElement.InnerHtml);
                    var word = wordElement.InnerText.Trim();
                    var deleni = "placeholder";
                    var rod = "placeholder";

                    if (hlavickaElement.InnerHtml.Contains("polozky"))
                    {
                        //if there is some shit instead of Rod

                        // Dělení
                        var deleniElement = hlavickaElement.NextSibling.NextSibling;
                        deleni = deleniElement.InnerText.Trim();

                        // Rod
                        var rodElement = hlavickaElement.NextSibling.NextSibling.NextSibling.NextSibling;
                        rod = rodElement.InnerText.Trim();
                    }
                    else
                    {
                        //if normal

                        // Dělení
                        var deleniElement = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='polozky']").FirstChild;
                        deleni = deleniElement.InnerText.Trim();

                        // Rod
                        var rodElement = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='polozky']").NextSibling;
                        rod = rodElement.InnerText.Trim();
                    }

                    var tableElement = htmlDocument.DocumentNode.SelectNodes("//tr");

                    //TABLE
                    string[] infoRow = []; //1row
                    string[] firstPadRow = []; //2row
                    string[] secondPadRow = []; //3row
                    string[] thirdPadRow = []; //4row
                    string[] fourthPadRow = []; //5row
                    string[] fifthPadRow = []; //6row
                    string[] sixthPadRow = []; //7row
                    string[] seventhPadRow = []; //8row

                    byte currentRow = 1;
                    foreach (var tr in tableElement)
                    {
                        var firstColumn = tr.FirstChild.InnerText.Trim();
                        var secondColumn = tr.FirstChild.NextSibling.InnerText.Trim();
                        var thirdColumn = tr.FirstChild.NextSibling.NextSibling.InnerText.Trim();

                        switch (currentRow)
                        {
                            case 1:
                                infoRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 2:
                                firstPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 3:
                                secondPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 4:
                                thirdPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 5:
                                fourthPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 6:
                                fifthPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 7:
                                sixthPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                            case 8:
                                seventhPadRow = [firstColumn, secondColumn, thirdColumn];
                                break;
                        }
                        currentRow++;
                    }

                    string[] firstPadSingular = firstPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] firstPadPlural = firstPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] secondPadSingular = secondPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] secondPadPlural = secondPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] thirdPadSingular = thirdPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] thirdPadPlural = thirdPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] fourthPadSingular = fourthPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] fourthPadPlural = fourthPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] fifthPadSingular = fifthPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] fifthPadPlural = fifthPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] sixthPadSingular = sixthPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] sixthPadPlural = sixthPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] seventhPadSingular = seventhPadRow[1].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");
                    string[] seventhPadPlural = seventhPadRow[2].TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9').Split(", ");


                    getJsonSubstantivaData();
                    void getJsonSubstantivaData()
                    {
                        var wordJson = new wordData
                        {
                            ID = Variables.ID++,
                            druh = "substantiva",
                            slovo = word.TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9'),
                            deleni = deleni.Remove(0, 8).TrimEnd('1', '2', '3', '4', '5', '5', '6', '7', '8', '9'),
                            rod = rod.Remove(0, 5).TrimEnd('.'),

                            firstPadSingular = firstPadSingular,
                            firstPadPlural = firstPadPlural,
                            secondPadSingular = secondPadSingular,
                            secondPadPlural = secondPadPlural,
                            thirdPadSingular = thirdPadSingular,
                            thirdPadPlural = thirdPadPlural,
                            fourthPadSingular = fourthPadSingular,
                            fourthPadPlural = fourthPadPlural,
                            fifthPadSingular = fifthPadSingular,
                            fifthPadPlural = fifthPadPlural,
                            sixthPadSingular = sixthPadSingular,
                            sixthPadPlural = sixthPadPlural,
                            seventhPadSingular = seventhPadSingular,
                            seventhPadPlural = seventhPadPlural
                        };
                        string jsonString = JsonSerializer.Serialize(wordJson);
                        SaveJson(word, jsonString, wordJson.ID);
                    }
                }

                static void SaveJson(string word, string jsonString, int? ID)
                {
                    string IDstring = Convert.ToString(ID);
                    using (FileStream savejson = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"output\" + IDstring + "_" + word + ".mrfjson", FileMode.Create))
                    {
                        byte[] InBinary = System.Text.Encoding.Default.GetBytes(jsonString);
                        savejson.Write(InBinary);
                    }
                }



            }
        }

        public class slovoSKUinfo 
        {
            public Dictionary<string, SKUinfo>? Slovos { get; set; }
        }
        public class SKUinfo
        {
            public int? ID { get; set; }
            public string Druh { get; set; }
            public string Rod { get; set; }
            public string SlovoPath { get; set; }
        }


        int currentPosInSlovosArray = 0;
        public class Slovos
        {

        }
        static void GenSku()
        {
            Dictionary<string, Slovos>[] slovosDataArray;

            string[] mrfSlovos = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"output\", "*.mrfjson");
            foreach (string mrfSlovo in mrfSlovos)
            {
                string jsonString = System.IO.File.ReadAllText(mrfSlovo);

                wordData? mrfJson =
                JsonSerializer.Deserialize<wordData>(jsonString);


                var skuInfo = new SKUinfo
                {
                    ID = mrfJson.ID,
                    Druh = mrfJson.druh,
                    Rod = mrfJson.rod,
                    SlovoPath = mrfSlovo
                };


                //wordData info = new wordData();
                Console.WriteLine(mrfJson.slovo);
                Console.ReadKey();
            }

            void writeToDictionary()
            {
                slovosDataArray[0] = SKUinfo;
            }

        }
    }

}