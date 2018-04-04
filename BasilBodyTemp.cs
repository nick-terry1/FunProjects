using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data;
using System.Data.Linq.Mapping;
using System.Diagnostics;

// title: BasilBodyTempProg
// author: Nick Terry

/* Description: This program will record the users basil body temperature and store the data in a database named babies.
 * The database table is called Temps and has two columns - dateEntered which is the primary key, and temperature which is a double
 * value. The program allows for the user to input a temperature, delete the last temperature input (in the case of accidentally 
 * entering the wrong value), delete the entire table and start over from the beginning, and query the data to find
 * the dates that the user is ovulating which is the main purpose of this program.
 * Sources that led to the math used in the GetData method are at:
 * www.fertilityplus.com/faq/bbt/bbtfaq.html, and https://www.wikihow.com/Take-Your-Basal-Body-Temperature */

namespace babies2
{
    class Program
    {
        static void Main(string[] args)
        {
            // connecting to the database babies, and linking the table 'Temps'
            babiesDataContext db = new babiesDataContext();
            Table<Temps> Temps = db.GetTable<Temps>();

           
                // The title and author of the program displayed for 6 seconds
                Stopwatch intro = new Stopwatch();
                intro.Start();
                Console.WriteLine("******************************************************\n" +
                    "******************************************************\n" +
                    "*                Basil Body Temperature              *\n" +
                    "*                   Tracking Program                 *\n" +
                    "*                                                    *\n" +
                    "******************************************************\n" +
                    "******************************************************");
                while (intro.Elapsed < TimeSpan.FromSeconds(3))
                { }
                Console.Clear();
                Console.WriteLine("******************************************************\n" +
                    "******************************************************\n" +
                    "*                                                    *\n" +
                    "*                CREATED BY NICK TERRY               *\n" +
                    "*                                                    *\n" +
                    "******************************************************\n" +
                    "******************************************************");
                while (intro.Elapsed < TimeSpan.FromSeconds(6))
                { }
                intro.Stop();
                MainMenu();

                void MainMenu()
                {
                try { 
                    ConsoleKeyInfo mmOption;

                    Console.Clear();
                    Console.WriteLine("Pick an option:\n" +
                        "1) Enter new temperature\n" +
                        "2) Access Data\n" +
                        "3) Erase all data and start over\n" +
                        "4) Delete the last entry\n");
                    Console.Write("Press <esc> to quit the program.\n");

                    mmOption = Console.ReadKey();
                    char charOption = mmOption.KeyChar;

                    // quits program with the escape key
                    if (mmOption.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else
                    {
                        switch (charOption)
                        {
                            case '1':
                                {
                                    NewTemp();
                                    MainMenu();
                                    break;
                                }
                            case '2':
                                {
                                    GetData();
                                    MainMenu();
                                    break;
                                }
                            case '3':
                                {
                                    NewTable();
                                    MainMenu();
                                    break;
                                }
                            case '4':
                                {
                                    DeleteLast();
                                    MainMenu();
                                    break;
                                }
                            // catches any input that isnt 1-4 or esc
                            default:
                                {
                                    Console.WriteLine("Incorrect input, enter one of the menu option numbers");
                                    MainMenu();
                                    break;
                                }
                        }
                    }
                }
                // catches anyone trying to enter two teperatures on the same day.
                catch (System.Data.Linq.DuplicateKeyException)
                {
                    Console.WriteLine("You have already entered a temperature today, only one temperature input a day allowed...\n" +
                        "Press <ENTER>  to exit the program.");
                    Console.ReadLine();
                    return;
                }

                // the only thing that can throw this exception is attempting to enter duplicate values as the date. 
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("You have already entered a temperature today, only one temperature input a day allowed...\n" +
                        "press <ENTER> to exit the program.");
                    Console.ReadLine();
                    return;
                }

                catch (System.FormatException fEx)
                {
                    Console.WriteLine(fEx.Message);
                    Console.WriteLine("press <ENTER>");
                    Console.ReadLine();
                    MainMenu();
                    return;
                }

                // if the user tries to erase the last entry when there are no entries this exception is thrown.
                catch (System.InvalidOperationException)
                {
                    Console.WriteLine("There are no entries to erase. Press <ENTER> to exit the program.");
                    Console.ReadLine();
                    return;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press <ENTER> to exit the program.");
                    Console.ReadLine();
                    return;
                }

                void NewTemp()
                {
                    Console.Clear();
                    Console.WriteLine("Enter a new temperature");
                    float usertemp = float.Parse(Console.ReadLine());

                    // the current date is the primary key, saved down to the day.
                    Temps newInsert = new Temps() { dateEntered = DateTime.Now.Date, temperature = usertemp };
                    db.Temps.InsertOnSubmit(newInsert);
                    db.SubmitChanges();
                    Console.WriteLine("Today's temperature has been saved! Press <ENTER> to return to the Main Menu.");
                    Console.ReadLine();
                    return;
                }

                void DeleteLast()
                {
                    Console.Clear();
                    Console.WriteLine("Are you sure you want to delete your last entry? (Y/N)");
                    char ans = Console.ReadKey().KeyChar;
                    if (ans == 'y')
                    {
                        // converting the result of this query to a list and saving it in fullList
                        var fullList = (from del in Temps
                                        where del.dateEntered != null
                                        select del.dateEntered).ToList();

                        // the highest date value (the newest value) is the one we want to delete.
                        // If fullList is empty fullList.Max() method will throw InvalidOperationException.
                        DateTime lastDate = fullList.Max();

                        // 'delete' is the row where the value in the dateEntered column is equal to the lastDate value.
                        var delete = from d in Temps
                                     where d.dateEntered == lastDate
                                     select d;

                        db.Temps.DeleteAllOnSubmit(delete);
                        db.SubmitChanges();
                        Console.WriteLine("\nLast entry was deleted. Press <ENTER> to return to Main Menu");
                        Console.ReadLine();
                        return;
                    }

                    else if (ans == 'n')
                    {
                        return;
                    }

                    else
                    {
                        Console.WriteLine("\nThat was not an option. Only type the key Y or N.\n" +
                            "Press <ENTER> to return to the Main Menu.");
                        Console.ReadLine();
                        return;
                    }
                }

                void NewTable()
                {
                    Console.Clear();
                    Console.WriteLine("Are you sure you want to delete everything and start over? (Y/N)");
                    ConsoleKeyInfo ans = Console.ReadKey();

                    if (ans.Key == ConsoleKey.Y)
                    {
                        // this query returns all entries because everything is less than or equal to DateTime.Now.
                        var remove = from r in Temps
                                     where (r.dateEntered <= DateTime.Now)
                                     select r;
                        db.Temps.DeleteAllOnSubmit(remove);
                        db.SubmitChanges();
                        Console.WriteLine("\nAll data has been erased. Press <ENTER> to return to Main Menu.");
                        Console.ReadLine();
                        return;
                    }

                    else if (ans.Key == ConsoleKey.N)
                    {
                        return;
                    }

                    else
                    {
                        Console.WriteLine("That wasn't a valid option, only excepting Y or N.\n" +
                            "Press <ENTER> to return to the Main Menu.");
                        Console.ReadLine();
                        return;
                    }
                }

                void GetData()
                {
                    Console.Clear();
                    
                    // all the dates in the dateEntered column is transfered to an array - dateArray
                    DateTime[] dateArray = (from d in Temps
                                            select d.dateEntered).ToArray();

                    // all the temperatures in the temperature column is transfered to an array - tempArray
                    double[] tempArray = (from t in Temps
                                          select t.temperature).ToArray();

                    // defining understandable variables to the array lengths and average temperature
                    double avgTemp = tempArray.Average();
                    int length = tempArray.Length;

                    // there has to be a minimum number of 30 entries to get a reliable answer according to sources listed in description.
                    if (tempArray.Length > 30)
                    {
                        // if your temperature is above the the average, odds are you are ovulating, but to make sure it
                        // isnt just a fluke we need to test for three consecutive days.
                        if ((tempArray[length - 1] > avgTemp) && (tempArray[length - 2] > avgTemp) && (tempArray[length - 3] > avgTemp))
                        {
                            double ovTemp;
                            int count = length;

                            // this loop finds the last date that the BBT dropped below the average.
                            do
                            {
                                count--;
                                ovTemp = tempArray[count];
                            }
                            while (ovTemp > avgTemp);
                            DateTime ovDate = dateArray[count];

                            // display date in long date format
                            Console.WriteLine("You are currently ovulating.\n" +
                                "The date you started ovulation on was {0:D}.\n" +
                                "Press <ENTER> to return to the Main Menu.", ovDate);
                            Console.ReadLine();
                        }

                        else
                        {
                            // this code loop will find the date of the users last ovulation by incrementing through the last two dates
                            // that are above the average. Need at LEAST 48 hours above average temp for accuracy.
                            int count = length;
                            for (int j = length - 1; ((tempArray[j] < avgTemp) && (tempArray[j - 1] < avgTemp)); j--)
                            {
                                count--;
                            }
                            DateTime lastOV = dateArray[count];
                            Console.WriteLine("You are not currently ovulating.\n" +
                                "The date of your last ovulation was {0:D}.\n" +
                                "Press <ENTER> to return to the Main Menu...", lastOV);
                            Console.ReadLine();
                        }
                    }

                    else
                    {
                        Console.WriteLine("You need at least 30 days of data for an accurate assessment.\n" +
                            $"Right now you have only {length} days of inputting temperatures.\n" +
                            $"Try again in {30 - length} days.\n" +
                            $"Press <ENTER>");
                        Console.ReadLine();
                    }
                }
            }
            
        }
    }
}