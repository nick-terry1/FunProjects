using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Author: Nick Terry
// Date: March 10, 2018

//************************************** DESCRIPTION ****************************************************
/* This program is designed to calculate the amount of dry and wet food to feed your cat on a daily     *
 * basis. It assumes the average calorie content of a cup of dry food to be 300 kcal per cup and        *
 * 250 kcal per 6oz can wet food. The formula and daily requirements were based on the National         *
 * Research Council (NRC) 2006 report on Nutrient Requirements for Cats and Dogs. Average calorie       *
 * values were taken from Dunloggin Veterinary Hospital website:                                        *
 * https://dunlogginvet.com/how-many-calories-should-your-dog-or-cat-eat-daily/                         *
 * NRC info can be found at:                                                                            *
 * https://www.merckvetmanual.com/management-and-nutrition/nutrition-small-animals/nutritional-         *
 * requirements-and-related-diseases-of-small-animals#v3326268                                          *
 * *****************************************************************************************************/

namespace CatFood
{
    class Program
    {
        static void Main(string[] args)
        {
            double lb;
            // Formula uses kg, so to convert - 1lb = 2.20462262 kg
            double kg = 2.20462262;
            double weight;
            // MER stands for Maintenance Energy Requirement
            double MER;
            // RER stands for Resting Energy Requirement
            double RER;
            // average calories of dry and wet food per cup and can
            double dry = 300;
            double wet = 250;

            START:
            try
            { 
                // Formula takes into consideration age (roughly), neutered or not, and whether he is obese prone or not
                Console.WriteLine("Is your cat less than one year old? Y/N: ");
                char age = Console.ReadKey().KeyChar;

                if ((age == 'y') || (age == 'Y'))
                {
                    KittenCalc();
                    return;
                }

                else if ((age == 'n') || (age == 'N'))
                {
                    AdultCalc();
                    return;
                }

                else
                {
                    Console.WriteLine("That was not a valid answer. Input only Y or N");
                    goto START;
                }

            }

            catch (FormatException fEx)
            {
                Console.WriteLine($"{fEx.Message}\n" +
                    $"Try again with correct inputs");
                goto START;
            }

            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.ReadLine();
            }

            void KittenCalc()
            {
                // user inputs kittens weight and converts it to kilograms
                Console.WriteLine(" ");
                Console.WriteLine("Please enter the weight of your kitten in pounds: ");
                lb = int.Parse(Console.ReadLine());
                weight = lb / kg;
                // RER calculated by 70[weight in kg^0.75]
                RER = 70 * (Math.Pow(weight, .75));
                // The constant in the MER calculation changes based on age, neutered, or obese variables
                MER = 2.5 * RER;

                double dailyDry = MER / dry;
                double dailyWet = MER / wet;

                // Formated to a tenth of a cup/can
                Console.WriteLine(" ");
                Console.WriteLine("The daily intake requirement for your kitten is {0:F1} cups of dry food\n" +
                    "and {1:F1} cans of wet food (where each can is 6oz).", dailyDry, dailyWet);
                Console.ReadLine();
                return;
            }

            // Program for kitten is repeated for adult cat with more conditionals and slight formula variations.
            void AdultCalc()
            {
                Console.WriteLine(" ");
                Console.WriteLine("Is your cat obese or prone to obesity? Y/N");
                char obese = Console.ReadKey().KeyChar; 
                if ((obese == 'y') || (obese == 'Y'))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter the weight of your cat in pounds: ");
                    lb = int.Parse(Console.ReadLine());
                    weight = lb / kg;
                    RER = 70 * (Math.Pow(weight, .75));
                    MER = 1.0 * RER;

                    double dailyDry = MER / dry;
                    double dailyWet = MER / wet;

                    Console.WriteLine(" ");
                    Console.WriteLine("The daily intake requirement for your cat is {0:F1} cups of dry food\n" +
                        "and {1:F1} cans of wet food (where each can is 6oz).", dailyDry, dailyWet);
                    Console.ReadLine();
                    return;
                }

                else if ((obese == 'n') || (obese == 'N'))
                {
                    Console.WriteLine("Is your cat neutered? Y/N: ");
                    char neutered = Console.ReadKey().KeyChar;

                    if ((neutered == 'y') || (neutered == 'Y'))
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine("Please enter the weight of your cat in pounds: ");
                        lb = int.Parse(Console.ReadLine());
                        weight = lb / kg;
                        RER = 70 * (Math.Pow(weight, .75));
                        MER = 1.2 * RER;

                        double dailyDry = MER / dry;
                        double dailyWet = MER / wet;

                        Console.WriteLine(" ");
                        Console.WriteLine("The daily intake requirement for your cat is {0:F1} cups of dry food\n" +
                            "and {1:F1} cans of wet food (where each can is 6oz).", dailyDry, dailyWet);
                        Console.ReadLine();
                        return;
                    }
                    else if ((neutered == 'n') || (neutered == 'N'))
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine("Please enter the weight of your cat in pounds:");
                        lb = int.Parse(Console.ReadLine());
                        weight = lb / kg;
                        RER = 70 * (Math.Pow(weight, .75));
                        MER = 1.4 * RER;

                        double dailyDry = MER / dry;
                        double dailyWet = MER / wet;

                        Console.WriteLine(" ");
                        Console.WriteLine("The daily intake requirement for your cat is {0:F1} cups of dry food\n" +
                            "and {1:F1} cans of wet food (where each can is 6oz).", dailyDry, dailyWet);
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("That was not a valid answer. Answer only Y or N.");
                        AdultCalc();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("That was not a valid answer. Answer only Y or N.");
                    AdultCalc();
                    return;
                }
            }
        }
    }
}
