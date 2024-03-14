# calc-challenge
String Calculator

* Enter a series of numbers separated by a comma (,) or newline character (\n) to find the sum of the numbers.
* By default negative numbers are not allowed, and the maximum number that can be entered is 1000.
* Custom delimiters may be defined as follows:
  * By using a single character after two forward slashes. Ex: //#,2,3#4,5#6
  * By using a series of brackets after two forward slashes to contain the delimiters. Ex //[##][#*#],2,3\n4,5
* The Config.json file in the project contains settings to further allow you to customize the following behavior.
    * Maximum Digit Size
    * To allow negative values
    * The Maximum amount of values to be used in calculations.
    * The default delimiters accepted
