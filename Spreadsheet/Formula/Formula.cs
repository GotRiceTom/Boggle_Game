// Skeleton written by Joe Zachary for CS 3500, January 2017
// Filled in by Eric Naegle u0725372 1/24/2018

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        private string formulaString;

        //previous, and the number of parenthesis, is necessary for formula formatting
        private int openParens, closeParens;
        private string previous;
        private List<string> normalizedTokens;
        private ISet<string> variableSet;

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        /// 

        //this constructor is used if there is nothing but a string passed in so no normalizing or validating is done
        public Formula(String theFormula) : this(theFormula, s => s, t => true){}

        //this one is used if a normalizer and validator is passed in
        public Formula(String theFormula, Normalizer N, Validator V)
        {
            if (theFormula == null || N == null || V == null)
                throw new ArgumentNullException("The parameter cannot be null");

            //initialize the fields for the struct
            string formula = theFormula;
            openParens = 0;
            closeParens = 0;
            variableSet = new HashSet<string>();

            //this will be used in the Evaluate method
            normalizedTokens = new List<string>();

            //I have to initialize this to something.
            this.previous = null;

            //check if the parameter is null or whitespace
            if (string.IsNullOrWhiteSpace(formula))
                throw new FormulaFormatException("The formula cannot be null or whitespace");

            //saved for the Evaluate function
            this.formulaString = formula;

            //get tokens ready
            var tokens = GetTokens(formula);

            bool isFirstVariable = true;
            foreach (string enumeratedToken in tokens)
            {
                string token = enumeratedToken;

                //split the string into characters for testing 
                char[] letters = token.ToCharArray();

                //this is where I do all of the normalizer and validator checks, and keep track of all of the normalized variables
                if (char.IsLetter(letters[0]))
                {
                    bool isVariable = true;

                    foreach (char letter in letters)
                    {
                        if (!(char.IsLetterOrDigit(letter)))
                            isVariable = false;
                    }

                    if (isVariable)
                    {
                        token = N(token);
                        if (!V(token))
                            throw new FormulaFormatException("The validator didn't like " + token);
                        variableSet.Add(token);
                    }
                }

                //the letters that we are checking need to be changed to the normalized letters
                letters = token.ToCharArray();

                //check if it's an operator or a single-character variable or digit.
                if (letters.Length == 1)
                {
                    if (!((token == "+") || (token == "-") || (token == "(") || (token == ")") || (token == "*") || (token == "/") || (char.IsLetterOrDigit(letters[0]))))
                    {
                        throw new FormulaFormatException(token + " is invalid.");
                    }

                    if (token == "(")
                        openParens++;

                    if (token == ")")
                        closeParens++;
                }

                //check if it's a longer variable like x5 or x64y8
                else if (char.IsLetter(letters[0]))
                {
                    foreach (char letter in letters)
                    {
                        if (!(char.IsLetterOrDigit(letter)))
                        {
                            throw new FormulaFormatException(token + " is invalid. Variables should start with a letter and be followed only by letters or digits.");
                        }

                        variableSet.Add(token);
                    }
                }

                // check if it's a double
                else if (char.IsDigit(letters[0]))
                {
                    if (!(double.TryParse(token, out double unused2)))
                    {
                        throw new FormulaFormatException(token + " is invalid.");
                    }
                }

                if (isFirstVariable == false)
                {
                    //make sure opening parenthesis and operators are followed by the right thing.
                    if ((previous == "(") || (previous == "+") || (previous == "-") || (previous == "*") || (previous == "/"))
                    {
                        if ((token == "*") || (token == "/") || (token == "+") || (token == "-") || (token == ")"))
                            throw new FormulaFormatException("An opening paranthesis must be followed by a number, variable, or opening parenthesis");
                    }

                    //make sure things that follow numbers, variables, or closing parenthesis are the right thing.
                    if (!((previous == "*") || (previous == "/") || (previous == "+") || (previous == "-") || (previous == "(")))
                    {
                        if (!(((token == "+") || (token == "-") || (token == "*") || (token == "/") || (token == ")"))))
                            throw new FormulaFormatException("A number, variable, or closing parenthesis must be followed by an operator or another closing parenthesis");
                    }
                }

                //check first token
                if (isFirstVariable == true)
                {
                    if ((token == ")") || (token == "*" || (token == "+") || (token == "-") || token == "/"))
                        throw new FormulaFormatException("The first token cannot be a close parenthesis or operator.");
                    isFirstVariable = false;
                }

                //I created a new list of all of the tokens, except this time the variables have been normalized and checked for validity
                normalizedTokens.Add(token);

                //hold on to the previous to check validity of next object
                previous = token;
            }

            //check final token in the formula
            if ((previous == "(") || (previous == "*" || (previous == "+") || (previous == "-") || previous == "/"))
                throw new FormulaFormatException("The last token cannot be an opening parenthesis or operator.");

            //check final number of parenthesis
            if (!(openParens == closeParens))
                throw new FormulaFormatException("Number of open and closing parenthesis don't match.");
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            if (lookup == null)
            {
                throw new ArgumentNullException("Parameter cannot be null.");
            }

            //This is how I make sure that the zero argument constructor works
            if (normalizedTokens == null)
                normalizedTokens = new List<string>{"0"};

            //create the fields necessary
            Stack<double> values = new Stack<double>();
            Stack<String> operators = new Stack<String>();
            openParens = 0;
            closeParens = 0;

            //this loops through the list of tokens that were normalized in the constructor.
            foreach(string token in normalizedTokens)
            {
                //if the token is a variable
                char[] letters = token.ToCharArray();
                if (char.IsLetter(letters[0]))
                {
                    //add it to the variable set
                    variableSet.Add(token);

                    double tokenDouble = lookup(token);
                    if ((operators.Count == 0) || (operators.Peek() == "+") || (operators.Peek() == "-") || (operators.Peek() == "(") || operators.Peek() == ")")
                    {
                        values.Push((tokenDouble));
                    }

                    else if (operators.Peek() == "*")
                    {
                        double product = values.Pop() * (tokenDouble);
                        operators.Pop();
                        values.Push(product);
                    }

                    else if (operators.Peek() == "/")
                    {
                        if (tokenDouble == 0.0)
                            throw new FormulaEvaluationException("You cannot divide by zero.");

                        double quotient = values.Pop() / (tokenDouble);
                        operators.Pop();
                        values.Push(quotient);
                    }
                }

                //if the token is a double
                else if (double.TryParse(token,out double unused2))
                {
                    if ((operators.Count == 0) || (operators.Peek() == "+") || (operators.Peek() == "-") || (operators.Peek() == "(") || operators.Peek() == ")")
                    {
                        values.Push(double.Parse(token));
                    }

                    else if (operators.Peek() == "*")
                    {
                        double product = values.Pop() * double.Parse(token);
                        operators.Pop();
                        values.Push(product);
                    }

                    else if (operators.Peek() == "/")
                    {
                        if (double.Parse(token) == 0.0)
                            throw new FormulaEvaluationException("You cannot divide by zero.");

                        double quotient = values.Pop() / double.Parse(token);
                        operators.Pop();
                        values.Push(quotient);
                    }
                }

                //if the token is a *, /, or (, we just push it onto the stack
                else if ((token == "*") || (token == "/"))
                {
                    operators.Push(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                    openParens++;
                }

                //if the token is a + or -
                else if ((token == "+") || (token == "-"))
                {
                    if (operators.Count == 0)
                        operators.Push(token);

                    else if (operators.Peek() == "+")
                    {
                        double second = values.Pop();
                        double first = values.Pop();

                        operators.Pop();
                        values.Push(first + second); 
                    }

                    else if (operators.Peek() == "-")
                    {
                        double second = values.Pop();
                        double first = values.Pop();

                        operators.Pop();
                        values.Push(first - second);
                    }

                    operators.Push(token);
                }

                //if the token is a ")"
                else if ((token == ")"))
                {
                    closeParens++;
                    if (closeParens > openParens)
                        throw new FormulaFormatException("There are close parenthesis for which there are no open parenthesis.");

                    if (operators.Peek() == "+")
                    {
                        double second = values.Pop();
                        double first = values.Pop();

                        operators.Pop();
                        values.Push(first + second);
                    }

                    //...in hindsight I should have learned how to use switch statements...
                    //I'll do it next time.
                    else if (operators.Peek() == "-")
                    {
                        double second = values.Pop();
                        double first = values.Pop();

                        operators.Pop();
                        values.Push(first - second);
                    }

                    operators.Pop();

                    if ((operators.Count > 0) && (operators.Peek() == "*"))
                    {
                        double product = values.Pop() * values.Pop();
                        operators.Pop();
                        values.Push(product);
                    }

                    else if ((operators.Count > 0) && (operators.Peek() == "/"))
                    {
                        double second = values.Pop();
                        double first = values.Pop();

                        if (second == 0)
                            throw new FormulaEvaluationException("You tried to divide by zero.");

                        operators.Pop();
                        values.Push(first/second);
                    }
                }
            }

            //at the very end, return the value if we're done...
            if (operators.Count == 0)
                return values.Pop();

            //...otherwise do the last bit of math and then return.
            else
            {
                double second = values.Pop();
                double first = values.Pop();

                if (operators.Peek() == "+")
                    values.Push(first+second);

                else if (operators.Peek() == "-")
                    values.Push(first-second);

                return values.Pop();
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            // NOTE:  These patterns are designed to be used to create a pattern to split a string into tokens.
            // For example, the opPattern will match any string that contains an operator symbol, such as
            // "abc+def".  If you want to use one of these patterns to match an entire string (e.g., make it so
            // the opPattern will match "+" but not "abc+def", you need to add ^ to the beginning of the pattern
            // and $ to the end (e.g., opPattern would need to be @"^[\+\-*/]$".)
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Gives back a set of all of the variables in the formula after they have been normalized and validated.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetVariables()
        {
            if (variableSet == null)
                return new HashSet<string>();

            return variableSet;
        }

        /// <summary>
        /// This overrides the ToString method. It will return the original formula string that was sent in as a parameter once the
        /// formula has gone through the constructor and had its variables normalized and validated.
        /// </summary>
        /// <returns> the original string but with the variables normalized </returns>
        public override string ToString()
        {
            if (normalizedTokens == null)
                normalizedTokens = new List<string> { "0" };

            string normalizedFormula = null;

            foreach (string token in normalizedTokens)
                normalizedFormula = normalizedFormula + token;
            
            return normalizedFormula;
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    public delegate string Normalizer(string s);
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
            throw new FormulaEvaluationException(variable + " is not valid.");
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
