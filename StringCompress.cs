using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// author: nick terry
// date: mar 23, 2018
/* Description: This program will compress a string of user input into a string that gives the character followed by
 * the number of times it was sequentially written for example aabcccccaaa would be a2b1c5a3. The string is modified
 * by using StringBuilder objects. If the string can't be compressed it returns the same string. For example abcdefg 
 * would return abcdefg, not a1b1c1d1e1f1g1. */

namespace StringCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            //aabcccccaaa would become a2b1c5a3
            Console.WriteLine("Enter string to compress");
            string test = Console.ReadLine();
            StringBuilder sb = new StringBuilder(test);

            // New StringBuilder object will be initialized to either its original string, sb, or its compressed form depending on
            // whether the method SameString() returns true or false.
            StringBuilder compressedString = (SameString(sb)) ? sb : Compress(sb);

            Console.WriteLine(compressedString);
            Console.ReadLine();

            // This method just tests to see if the two strings are the same or not and returns a boolean result
            bool SameString(StringBuilder s)
            {
                for (int j = 0; j < s.Length - 1; j++)
                {
                    if (s[j] == s[j + 1])
                    {
                        return false;
                    }
                }

                return true;
            }
            
            // This method compresses the string to the form described in the program description.
            StringBuilder Compress(StringBuilder s)
            {
                int count = 1;
                for (int i = 0; (i < s.Length); i++)
                {
                    //Console.WriteLine(s);
                    //Console.WriteLine(s[i]);              used for iteration testing
                    //Console.WriteLine(i);
                    //Console.ReadLine();                    
                    
                    // The count of the sequential characters is inserted into string builder if either the character at the first index
                    // and the second index are not the same XOR there is not sequential character to test. Short circuiting required here.
                    if ((i + 1 == s.Length) || (s[i] != s[i+1]))
                    {
                        s.Insert(i+1, count.ToString());
                        i++; // i incrimented to compensate for the inserted character
                        if (count > 9)
                        {
                            i++; // if a number two digits long is inserted incriment again - NOTE this program doesnt work for 
                        }        // cases where there are more than 99 identical characters in a row. I could always add more if statements though.
                        count = 1;      // count reinitialized to default
                    }
                    else if (s[i] == s[i + 1])
                    {
                        count++;
                        s.Remove(i, 1);
                        i--;        // i is decrimented to compensate for removed character
                    }
                }
                return s;
            }
        }
    }
}
