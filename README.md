# FunProjects
## Creation Date: March 13, 2018  
  
  ### Suggestions for any code improvement are welcome!

### Project CatFoodCalc

This program will calculate the amount of food that you should feed your cat based on the National Research Council's 2006 report titled "Nutrient 
Requirements for Cats and Dogs". That report can be found [here](https://www.merckvetmanual.com/management-and-nutrition/nutrition-small-animals/nutritional-requirements-and-related-diseases-of-small-animals#v3326268).
The average calorie content for a cup of cat food and a 6 oz. can of cat food was found [here](https://dunlogginvet.com/how-many-calories-should-your-dog-or-cat-eat-daily/).

### Project StringCompress  

This program takes a user input string and compresses it to a form that deletes all repeated sequential characters and inserts a number in its place. If the string can't be compressed it returns the original string. The algorithm has a complexity of O(n).
  
### Project BasilBodyTemp

This is for women who are trying to get pregnant by predicting when they are ovulating. The program does this by allowing the user to input their Basil Body
Temperature (BBT) once a day and finding trends in those values and outputting whether youre ovulating or the last date you were ovulating. It also lets the user 
delete data in case they want to start over or just accidentally input the wrong temperature. All data is inserted into a database and saved 
using LINQ and SQL Server. The information on reading trends in BBT came from two websites, 
[WikiHow.com](https://www.wikihow.com/Take-Your-Basal-Body-Temperature) and [FertilityPlus.com](http://www.fertilityplus.com/faq/bbt/bbtfaq.html). Credit Beth Terry for the program idea. 

### Project Song Analysis 

This program will take the lyrics from the top 100 songs overall, and top 100 country, hip hop, rock, and pop songs,
 gathered from the website [Metrolyrics.com](https://www.metrolyrics.com). All stop words (words like 'a' 'the' and 'and') are deleted along with all punctuation, 
 and numbers. It then stores the remaining words in a database along with their frequency of use and creates a word cloud bitmap
 image to display visually the top words used in the top 100 songs of today. 
  
 It also tests a randomly input song to see what genre of music it is (country, hip hop, pop, or rock only) based only on the 
 lyrics of that song by seperating all the words that are used exclusively with each genre, comparing those with the words of the 
 sample song, and seeing which genre it most closely matches.