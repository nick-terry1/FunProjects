using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using WordCloudGen = WordCloud.WordCloud;
using System.Drawing;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;

// author: nick terry
// date: may 18, 2018

/* description: this program will take the lyrics from the top 100 songs overall, and top 100 country, hip hop, rock, and pop songs,
 * gathered from the website Metrolyrics.com. All stop words (words like 'a' 'the' and 'and') are deleted along with all punctuation, 
 * and numbers. It then stores the remaining words in a database along with their frequency of use and creates a word cloud bitmap
 * image to display visually the top words used in the top 100 songs of today. 
 * 
 * It also tests a randomly input song to see what genre of music it is (country, hip hop, pop, or rock only) based only on the 
 * lyrics of that song by seperating all the words that are used exclusively with each genre, comparing those with the words of the 
 * sample song, and seeing which genre it most closely matches.
 * */

namespace testai
{
    class Program
    {
        static void Main(string[] args)
        {
            // websites for the top 100 of each genre, including the overall top 100
            string top100HTML = "http://www.metrolyrics.com/top100.html";
            string popHTML = "http://www.metrolyrics.com/top100-pop.html";
            string hiphopHTML = "http://www.metrolyrics.com/top100-hiphop.html";
            string rockHTML = "http://www.metrolyrics.com/top100-rock.html";
            string countryHTML = "http://www.metrolyrics.com/top100-country.html";

            List<KeyValuePair<string,int>> topList = GetTop100(top100HTML);
            GenerateWordCloud(topList);
            SaveTop100(topList);

            List<KeyValuePair<string, int>> popList = GetTop100(popHTML);
            GenerateWordCloud(popList);
            SavePop100(popList);

            List<KeyValuePair<string, int>> hiphopList = GetTop100(hiphopHTML);
            GenerateWordCloud(hiphopList);
            SaveHipHop100(hiphopList);

            List<KeyValuePair<string, int>> rockList = GetTop100(rockHTML);
            GenerateWordCloud(rockList);
            SaveRock100(rockList);

            List<KeyValuePair<string, int>> countryList = GetTop100(countryHTML);
            GenerateWordCloud(countryList);
            SaveCountry100(countryList);

            SaveExclusives();

            List<string> beHumble = GetRapSongLyrics(); // Kendrick Lamar
            TestSong(beHumble); // test outputs 72 matches with hip hop and 1 match with country and rock

            List<string> watermelonCrawl = GetCountrySongLyrics(); // Tracy Byrd
            TestSong(watermelonCrawl); // test outputs 3 country matches, 2 hiphop matches, and 1 rock match

        }
        public static List<KeyValuePair<string,int>> GetTop100(string webpage)
        {
            /* Scraping top 100 song lyrics */
            Dictionary<string, int> topWords = new Dictionary<string, int>();
            List<string> songs = new List<string>();
            WebClient web = new WebClient();
            WebClient web2 = new WebClient();

            // this regex collects all the website https that link to each song's lyrics 
            string html2 = web2.DownloadString(webpage);
            MatchCollection m2 = Regex.Matches(html2, @"<span class=.num.>.+?(http.+?html)", RegexOptions.Singleline);
            List<string> top100html = new List<string>();
            StringBuilder bestMatch = new StringBuilder();

            foreach (Match m in m2)
            {
                string h = m.Groups[1].Value;
                top100html.Add(h);
            }
            // iterating through each website collected in top100html list
            for (int i = 0; i < top100html.Count; i++)
            {
                bestMatch.Clear();
                string tempHTML = top100html[i];
                string html = web.DownloadString(tempHTML);

                // all lyrics were in paragraph tags which the unique identifier class='verse'
                MatchCollection m1 = Regex.Matches(html, @"<p class=.verse.>(.+?)</p>", RegexOptions.Singleline);

                // deleting all punctuation, numbers, etc.
                List<string> rm = new List<string>() { ",","'",".","<br>","!","?","\"","(",")","1","2","3","4","5",
                        "6","7","8","9","0","<",">","/","@","#","{","}","[","]","$","&","-","+","=",":","outro"};

                // cleaning up each word
                foreach (Match m in m1)
                {
                    string song = m.Groups[1].Value + " ";
                    songs.Add(song);
                    bestMatch.Append(song).Replace("\n", " ");
                }

                foreach (string rem in rm)
                {
                    bestMatch.Replace(rem, "");
                }

                // making sure the songs variable is empty before adding all the new word values
                songs.Clear();

                // seperating all the words by spaces, standardizing them to lowercase, trimming all whitespace, and converting to list
                songs = bestMatch.ToString().ToLower().Split(new[] { " " }, 
                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList<string>();

                // adding the frequency of use of each word and making it the key<string>'s value
                foreach (string s in songs)
                {
                    if (topWords.ContainsKey(s))
                    {
                        topWords[s]++;
                    }
                    else
                    {
                        topWords.Add(s, 1);
                    }
                }
            }

            // getting rid of all the stopwords in English, Spanish, and Indonesian
            WebClient stopWeb = new WebClient();
            string[] stopWebsites = new string[3] { "https://www.ranks.nl/stopwords",
                    "https://www.ranks.nl/stopwords/spanish", "https://www.ranks.nl/stopwords/indonesian" };

            List<string> stopWords = new List<string>();
            StringBuilder stopstring = new StringBuilder();

            for (int i = 0; i < stopWebsites.Length; i++)
            {
                // stopword lists were all in <td> tags with the valign='top' specification
                string stophtml = stopWeb.DownloadString(stopWebsites[i]);
                MatchCollection sm = Regex.Matches(stophtml, @"<td valign=.top.>.+?(.+?)</td>", RegexOptions.Singleline);

                foreach (Match m in sm)
                {
                    string tempstop = m.Groups[1].Value;
                    stopWords.Add(tempstop);
                    stopstring.Append(tempstop).Replace("/>", "");
                    stopstring.Replace("\n", " ");
                    stopstring.Replace("<br", " ");
                    stopstring.Replace("'", "");
                }
                stopWords = stopstring.ToString().ToLower().Split(new[] { " " },
                    StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList<string>();

                foreach (string erase in stopWords)
                {
                    if (topWords.ContainsKey(erase))
                    {
                        topWords.Remove(erase);
                    }
                }
            }

            // deleting non english words that had a significant count and werent deleted in the previous stopwords
            List<string> stopMisc = new List<string>() { "got", "tu", "di","ko","sa","tere","ang","ka","de","desi","ki","se","pa",
                "a","lang","ni","te","hai","na","bog","walang","kung","mo","teri","ne","sayo","mi","maa","ako","tera","naa","ng",
                "deewani","gayi","sila","valleyo","en","vi","mera","dil","ja","ke","je","bhi","es","le","ku","kau","ti","ei","semua",
                "kita","naman","yadain","bheegi","mein","ikaw","mong","sana","ba","koi","mga"};
            foreach (string erase in stopMisc)
            {
                if (topWords.ContainsKey(erase))
                {
                    topWords.Remove(erase);
                }
            }

            /* sorting the word list */
            List<KeyValuePair<string, int>> sortedList = topWords.ToList();
            sortedList.Sort((a, b) => a.Value.CompareTo(b.Value));
            //sortedList.Reverse();
            return sortedList;
        }

        public static void GenerateWordCloud(List<KeyValuePair<string, int>> sortedList)
        { 
            /* creating a WordCloud image */
            List<string> words = new List<string>();
            List<int> freq = new List<int>();

            // seperating the keyvalue pairs into the correct format of two lists
            foreach (KeyValuePair<string, int> e in sortedList)
            {
                words.Add(e.Key);
            }
            foreach (KeyValuePair<string, int> f in sortedList)
            {
                freq.Add(f.Value);
            }

            // 500, 500 refers to the image quality, true gives size based on freq, null gives words random colors, -1, 1 are step sizes
            var wc = new WordCloudGen(500, 500, true, null, -1, 1);
            // save image to solution's image folder
            wc.Draw(words, freq).Save(@"c:\users\nickt\source\repos\testai\testai\Images\wordcloud.bmp");
            Console.WriteLine("picture saved as wordcloud.bmp");

            foreach (KeyValuePair<string, int> test in sortedList)
            {
                Console.WriteLine(test);
            }
        }

        // saving the top 100 overall songs to table 'top100'
        public static void SaveTop100(List<KeyValuePair<string,int>> insertList)
        {
            SongsDataContext db = new SongsDataContext();
            Table<Top100> Songs = db.GetTable<Top100>();

            var remove = from r in Songs
                         select r;
            db.Top100s.DeleteAllOnSubmit(remove);

            foreach (KeyValuePair<string, int> lyric in insertList)
            {
                Top100 insertPair = new Top100() { word = lyric.Key, frequency = lyric.Value };
                db.Top100s.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
            Console.WriteLine("complete");
        }

        // saving top 100 pop song lyrics to 'pop' table
        public static void SavePop100(List<KeyValuePair<string, int>> insertList)
        {
            SongsDataContext db = new SongsDataContext();
            Table<Pop> Songs = db.GetTable<Pop>();

            var remove = from r in Songs
                         select r;
            db.Pops.DeleteAllOnSubmit(remove);
            db.SubmitChanges();

            foreach (KeyValuePair<string, int> lyric in insertList)
            {
                Pop insertPair = new Pop() { words = lyric.Key, frequency = lyric.Value };
                db.Pops.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
        }
        // saving the hip hop lyrics to table 'hiphop'
        public static void SaveHipHop100(List<KeyValuePair<string, int>> insertList)
        {
            SongsDataContext db = new SongsDataContext();
            Table<HipHop> Songs = db.GetTable<HipHop>();

            var remove = from r in Songs
                         select r;
            db.HipHops.DeleteAllOnSubmit(remove);

            foreach (KeyValuePair<string, int> lyric in insertList)
            {
                HipHop insertPair = new HipHop() { word = lyric.Key, frequency = lyric.Value };
                db.HipHops.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
        }

        // saving rock lyrics to table 'rock'
        public static void SaveRock100(List<KeyValuePair<string, int>> insertList)
        {
            SongsDataContext db = new SongsDataContext();
            Table<Rock> Songs = db.GetTable<Rock>();

            var remove = from r in Songs
                         select r;
            db.Rocks.DeleteAllOnSubmit(remove);

            foreach (KeyValuePair<string, int> lyric in insertList)
            {
                Rock insertPair = new Rock() { word = lyric.Key, frequency = lyric.Value };
                db.Rocks.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
        }
        // save country top lyrics to table 'country'
        public static void SaveCountry100(List<KeyValuePair<string, int>> insertList)
        {
            SongsDataContext db = new SongsDataContext();
            Table<Country> Songs = db.GetTable<Country>();

            // deleting all info in the country table before saving the new info
            var remove = from r in Songs
                         select r;
            db.Countries.DeleteAllOnSubmit(remove);

            // saving each value into the table where the key is the word and the value is the genre
            foreach (KeyValuePair<string, int> lyric in insertList)
            {
                Country insertPair = new Country() { word = lyric.Key, frequency = lyric.Value };
                db.Countries.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
        }

        // Method to save all the exclusively used words for each genre in the table 'exclusive words'
        public static void SaveExclusives()
        {
            SongsDataContext db = new SongsDataContext();
            Table<ExclusiveWord> exclusiveWords = db.GetTable<ExclusiveWord>();

            // creating hashsets made up of the top 100 words used in each genre and saved in their respective table
            Table<Pop> PopSongs = db.GetTable<Pop>();
            HashSet<string> popHash = new HashSet<string>((from x in PopSongs
                                                           orderby x.frequency descending
                                                           select x.words).Take<string>(100));

            Table<HipHop> HipHopSongs = db.GetTable<HipHop>();
            HashSet<string> hiphopHash = new HashSet<string>((from x in HipHopSongs
                                                              orderby x.frequency descending
                                                              select x.word).Take<string>(100));

            Table<Country> CountrySongs = db.GetTable<Country>();
            HashSet<string> countryHash = new HashSet<string>((from x in CountrySongs
                                                               orderby x.frequency descending
                                                               select x.word).Take<string>(100));

            Table<Rock> RockSongs = db.GetTable<Rock>();
            HashSet<string> rockHash = new HashSet<string>((from x in RockSongs
                                                            orderby x.frequency descending
                                                            select x.word).Take<string>(100));

            // creating hashsets with of each genre that holds only words that were only found in the top 100 songs of that genre
            HashSet<string> tempHash = new HashSet<string>(hiphopHash);

            tempHash.ExceptWith(rockHash);
            tempHash.ExceptWith(popHash);
            tempHash.ExceptWith(countryHash);
            HashSet<string> exclusiveHipHop = new HashSet<string>(tempHash);

            tempHash = new HashSet<string>(rockHash);

            tempHash.ExceptWith(hiphopHash);
            tempHash.ExceptWith(popHash);
            tempHash.ExceptWith(countryHash);
            HashSet<string> exclusiveRock = new HashSet<string>(tempHash);

            tempHash = new HashSet<string>(countryHash);

            tempHash.ExceptWith(hiphopHash);
            tempHash.ExceptWith(popHash);
            tempHash.ExceptWith(rockHash);
            HashSet<string> exclusiveCountry = new HashSet<string>(tempHash);

            tempHash = new HashSet<string>(popHash);

            tempHash.ExceptWith(hiphopHash);
            tempHash.ExceptWith(rockHash);
            tempHash.ExceptWith(countryHash);
            HashSet<string> exclusivePop = new HashSet<string>(tempHash);

            // deleting all previous entries in the exclusive words table to prepare for inputing new more current entries
            var remove = from r in exclusiveWords
                         select r;
            db.ExclusiveWords.DeleteAllOnSubmit(remove);

            // saving each word in the exclusive set lists to the table ExclusiveWord with its respective genre
            foreach (string w in exclusivePop)
            {
                ExclusiveWord insertPair = new ExclusiveWord() { word = w, genre = "pop" };
                db.ExclusiveWords.InsertOnSubmit(insertPair);
            }

            foreach (string w in exclusiveRock)
            {
                ExclusiveWord insertPair = new ExclusiveWord() { word = w, genre = "rock" };
                db.ExclusiveWords.InsertOnSubmit(insertPair);
            }
            foreach (string w in exclusiveHipHop)
            {
                ExclusiveWord insertPair = new ExclusiveWord() { word = w, genre = "hiphop" };
                db.ExclusiveWords.InsertOnSubmit(insertPair);
            }
            foreach (string w in exclusiveCountry)
            {
                ExclusiveWord insertPair = new ExclusiveWord() { word = w, genre = "country" };
                db.ExclusiveWords.InsertOnSubmit(insertPair);
            }
            db.SubmitChanges();
        }

        public static void TestSong(List<string> lyrics)
        {
            // these genre variables will hold the total word matches of the song
            int hiphop = 0, country = 0, rock = 0, pop = 0;

            SongsDataContext db = new SongsDataContext();
            Table<ExclusiveWord> exclusives = db.GetTable<ExclusiveWord>();
            Dictionary<string, string> genrecount = new Dictionary<string, string>();

            // object d is an anonymous type with fields word and genre - matching the columns of the Exclusive Words table.
            var d = (from x in exclusives
                      select new { x.word, x.genre });
            // storing these column values into a dictionary format
            foreach (var piece in d)
            {
                genrecount.Add(piece.word, piece.genre); 
            }

            // counting how many words of the song matches each genre's key words
            for (int i = 0; i < lyrics.Count; i++)
            {
                if (genrecount.ContainsKey(lyrics[i]))
                {
                    if (genrecount[lyrics[i]] == "hiphop")
                    {
                        hiphop++;
                    }
                    else if (genrecount[lyrics[i]] == "country")
                    {
                        country++;
                    }
                    else if (genrecount[lyrics[i]] == "rock")
                    {
                        rock++;
                    }
                    else if (genrecount[lyrics[i]] == "pop")
                    {
                        pop++;
                    }
                }
            }
            // shows how close the song is to each genre
            Console.WriteLine($"The song has {pop} pop matches, {hiphop} hip hop matches,\n" +
                $"{rock} rock matches, and {country} country matches");
        }

        // sole purpose of this method is to return the lyrics of a test song for the TestSong method
        public static List<string> GetRapSongLyrics()
        {
            // downloading the lyrics as a string from website songlyrics.com
            List<string> lyrics = new List<string>();
            StringBuilder lyricstring = new StringBuilder();
            WebClient web = new WebClient();
            string html = web.DownloadString(@"http://www.songlyrics.com/kendrick-lamar/humble-lyrics/");

            // all the lyrics are sandwiched between a paragraph tag with uniquely labeled class='songLyricsV14'
            Match m = Regex.Match(html, @"class=.songLyricsV14 iComment-text.>(.+?)</p>", RegexOptions.Singleline);

            // standardizing all characters to lowercase and removing all punctuation, numbers, etc.
            lyricstring.Append(m.Groups[1].ToString().ToLower());
            List<string> rm = new List<string>() { ",","'",".","!","?","\"","(",")","1","2","3","4","5",
                        "6","7","8","9","0","/","@","#","{","}","[","]","$","&","-","+","=",":","outro","<br","/>","<",">"};
            foreach (string remove in rm)
            {
                lyricstring.Replace(remove, "");
            }

            // splitting all words seperated by spaces and trimming all whitespace
            List<string> lyricList = lyricstring.ToString().Split(new[] { " " },
                StringSplitOptions.RemoveEmptyEntries ).Select(s => s.Trim()).ToList<string>();
            
            return lyricList;
        }
        // purpose of this method is to convert the lyrics of watermelon crawl to the correct format
        public static List<string> GetCountrySongLyrics()
        {
            string watermelonCrawl = "I was drivin' through Georgia in late July on a day hot enough to make the devil sigh I saw a homemade sign writtin' in " +
            "red rhine county watermelon festival ahead well, I wasn't in a hurry so I slowed down took a two lane road to " +
            "a one horse town there was a party goin' on when I got there I heard a welcome speech from a small town mayor He " +
            "said we got a hundred gallons of sweet red wine made from the biggest watermelons on the vine help yourself to some, " +
            "but obey the law if you drink don't drive do the watermelon crawl when the band started playin the watermelon queen " +
            "said let me show you somethin' that you ain't ever seen she grabbed me by the arm said come on lets go she dipped " +
            "down, spun around, and do-ce-doed she rocked back on her heels dropped down to her knees she craweled across the " +
            "floor and jumped back to her feet she wiggled and she giggled and be all you ever saw she said this is how you do " +
            "the watermelon crawl if your ever down in georgia around about july if you ain't in a hurry then you aw to stop by " +
            "i can guarantee that you're goin have a ball learnin' how to do the watermelon crawl yeah do the watermelon crawl " +
            "have fun you all do the watermelon crawl";

            // removing all punctuation, numbers, etc.
            List<string> rm = new List<string>() { ",","'",".","!","?","\"","(",")","1","2","3","4","5",
                        "6","7","8","9","0","/","@","#","{","}","[","]","$","&","-","+","=",":","outro","<br","/>","<",">"};
            foreach (string remove in rm)
            {
                watermelonCrawl.Replace(remove, "");
            }

            // seperating each word as its own List value and trimming all whitespace
            List<string> WatermelonCrawl = watermelonCrawl.ToLower().Split(new[] { " " },
                StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList<string>();
            return WatermelonCrawl;
        }
    }
}

