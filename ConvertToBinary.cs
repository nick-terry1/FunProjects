using System;
using System.Collections.Generic;
using System.IO;

/* author: Nick Terry
 * date: March 18, 2018
 * 
 * description: This program will take a user input base ten value and output its binary equivolent; Then it counts the max 
 * number of 1's in a row in the binary number. */

class Solution
{
    static void Main(String[] args)
    {
        Console.WriteLine("Enter the number to be converted to binary");
        int n = Console.ReadLine(); // user input base ten number
        string binary = ToBinary(n);
        Console.WriteLine("The number {0} in binary is {1}", n, binary);
        int k = binary.Length;
        int ones = OneCount(binary,k);
        Console.WriteLine("There are {0} ones in a row", ones);
        
    }

    // Method converts the base ten integer n to binary string finalStr
    public static string ToBinary(int n)
    {
        int r = n;
        int count = -1;
        string finalStr = ""; // The final binary number that I'll keep incrimenting with 1's and 0's until the end

        // This loop will determine the size of the base ten number by subtracting 2^i power until the value is negative
        for (int i = 0; r > 0; i++)
        {
            count++;
            r = n;
            r = n - ((int)Math.Pow(2, i));
        }

        // This loop finds the next highest value of n, inserts a '1' into the final binary string for it,
        // then checks for the next highest value, inserting 0's until it finds it
        while (count >= 0)
        {
            // First check if we're done and theres only '1' left.
            if ((n == 1) && (count == 0))
            {
                finalStr += "1";
                return finalStr;
            }

            // Subtract the highest power of 2 we found using the variable count, if that value is negative insert '0' to binary value
            else if (n - ((int)Math.Pow(2,count)) < 0)
            {
                finalStr += "0";
            }

            // If the highest power of 2 subtracted from n (the original number, before manipulation) is positive add '1' to binary value
            else
            {
                finalStr += "1";
                n -= (int)Math.Pow(2, count);
            }
            count--; // finally decriment count to move us down to the next power of 2
        }

        return finalStr;
        
    }

    // Method counts maximum number of 1's in a row in the string binary (the string result from 'ToBinary()')
    public static int OneCount(string binary, int k)
    {
        int i = 0;
        string compare = ""; // compare is the all 1's containing string that I'll check 'binary' against
        
        // this loop creates a string the size of binary that is all 1's
        while (i <= k)
        {
            compare += "1";
            i++;
        }
        
        // This if statement checks to see if the string contains the string compare inside it...
        if (binary.Contains(compare))
        {
            return i;
        }

        // ...if not, it creates another string of all 1's that is one size smaller until it finds a match
        else
        {
            return OneCount(binary, k - 1);
        }
    }
}