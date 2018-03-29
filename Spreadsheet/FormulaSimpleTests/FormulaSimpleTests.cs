// Written by Joe Zachary for CS 3500, January 2017.
// Extended by Eric Naegle u0725372 1/24/2018

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Collections.Generic;

namespace FormulaTestCases
{
    /// <summary>
    /// These are some of the tests supplied by the professor mixed with some of my own tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// Makes sure that a variable without the proper structure makes a formula exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadVariable()
        {
            Formula f = new Formula("x + 5 - y6)?5 + X45 - X", NormalizerAllCaps, ValidatorAllStringsAreCaps);
        }

        /// <summary>
        /// This makes sure that they normalizer works, that the validator lets a valid variable pass, and that toString works.
        /// </summary>
        [TestMethod()]
        public void ToString_And_All_Caps_Normalizer_And_All_Caps_Validator()
        {
            Formula f = new Formula("x + 5 - y65 + X45 - X", NormalizerAllCaps, ValidatorAllStringsAreCaps);
            string newFormula = f.ToString();
            if (newFormula != "X+5-Y65+X45-X")
                Assert.Fail();
        }

        /// <summary>
        /// This makes sure that they normalizer works, that the validator thows an exception on a bad variable, and that toString works.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ToString_And_All_Caps_Normalizer_And_All_Lower_Validator()
        {
            Formula f = new Formula("x + 5 - y65 + X45 - X", NormalizerAllCaps, ValidatorAllStringsAreLower);
        }

        /// <summary>
        /// Makes sure that a zero argument constructor works like "0".
        /// </summary>
        [TestMethod()]
        public void ZeroArgumentConstructor()
        {
            Formula f = new Formula();

            if (f.ToString() != "0" && f.Evaluate(Lookup4) != 0.0)
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a zero argument constructor works like "0".
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SingleNullArgumentConstructor()
        {
            Formula f = new Formula(null);
        }

        /// <summary>
        /// Checks that null parameters throw ArgumentNullExceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameter()
        {
            Formula f = new Formula(null, NormalizerDoesNothing, ValidatorDoesNothing);
        }

        /// <summary>
        /// Checks that null parameters throw ArgumentNullExceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameter2()
        {
            Formula f = new Formula("x", null, ValidatorDoesNothing);
        }

        /// <summary>
        /// Checks that null parameters throw ArgumentNullExceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameter3()
        {
            Formula f = new Formula("x", NormalizerDoesNothing, null);
        }

        /// <summary>
        /// Checks that null parameters throw ArgumentNullExceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParameterEvaluate()
        {
            Formula f = new Formula("5 + 6", NormalizerDoesNothing, ValidatorDoesNothing);
            f.Evaluate(null);
        }

        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("*5");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        public void Construct4()
        {
            Formula f = new Formula("((2+4)*x5)");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("-5+3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("(5+3))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("?");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1minus()
        {
            Formula f = new Formula("3-2");
            Assert.AreEqual(f.Evaluate(v => 0), 1.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate42()
        {
            Formula f = new Formula("x + y + z");
            Assert.AreEqual(f.Evaluate(Lookup4), 18.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Makes sure that "2*3" evaluates to 6.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate6()
        {
            Formula f = new Formula("2*3");
            Assert.AreEqual(f.Evaluate(v => 0), 6.0, 1e-6);
        }

        /// <summary>
        /// Makes sure that "8/2" evaluates to 4.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("8/2");
            Assert.AreEqual(f.Evaluate(v => 0), 4.0, 1e-6);
        }

        /// <summary>
        /// Makes sure that "8/2" evaluates to 4.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate8()
        {
            Formula f = new Formula("8/0");
            Assert.AreEqual(f.Evaluate(v => 0), 4.0, 1e-6);
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate9()
        {
            Formula f = new Formula("2+3+(5+7)-2+5");
            Assert.AreEqual(f.Evaluate(v => 0), 20.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate10()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 5.0, 1e-6);
        }

        /// <summary>
        /// I made this test to make sure that you can create two different
        /// formulas while still evaluating one properly.
        /// </summary>
        [TestMethod]
        public void Evaluate11()
        {
            Formula e = new Formula("( 10 * 3 ) / 5");
            Formula f = new Formula("2+3+(5+7)-2+5");
            Formula g = new Formula("( 10 * 3 ) / 5");
            Assert.AreEqual(f.Evaluate(v => 0), 20.0, 1e-6);
        }

        /// <summary>
        /// Testing parenthesis out of order, even though their totals match.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Evaluate12()
        {
            Formula f = new Formula("2+3)+(5+7)-(2+5");
            Assert.AreEqual(f.Evaluate(v => 0), 20.0, 1e-6);
        }

        [TestMethod]
        public void ZeroArg1()
        {
            new Formula();
        }

        [TestMethod]
        public void ZeroArg2()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.GetVariables().Count);
        }

        [TestMethod]
        public void ZeroArg3()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void ZeroArg4()
        {
            Formula f1 = new Formula();
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(0, f2.Evaluate(s => 1));
        }

        [TestMethod]
        public void ThreeArg1()
        {
            Formula f = new Formula("x+y", s => s, s => true);
            Assert.AreEqual(3, f.Evaluate(s => (s == "x") ? 1 : 2));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg2()
        {
            Formula f = new Formula("x+y", s => "$", s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg3()
        {
            Formula f = new Formula("x+y", s => s, s => false);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg4()
        {
            Formula f = new Formula("x+y", s => s == "x" ? "z" : s, s => s != "z");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg5()
        {
            Formula f = new Formula("$", s => "x", s => true);
        }

        [TestMethod]
        public void ThreeArg6()
        {
            Formula f = new Formula("1", s => "x", s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => { throw new UndefinedVariableException(""); }), 1e-6);
        }

        [TestMethod]
        public void ThreeArg7()
        {
            Formula f = new Formula("y", s => "x", s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => (s == "x") ? 1 : 0), 1e-6);
        }


        [TestMethod]
        public void ThreeArg8()
        {
            Formula f = new Formula("1e1 + e", s => "x", s => s == "x");
            Assert.AreEqual(12.0, f.Evaluate(s => (s == "x") ? 2 : 0), 1e-6);
        }

        [TestMethod]
        public void ThreeArg9()
        {
            Formula f = new Formula("xx+y", s => (s == "xx") ? "X" : "z", s => s.Length == 1);
            Assert.AreEqual(10.0, f.Evaluate(s => (s == "X") ? 7 : 3), 1e-6);
        }

        [TestMethod]
        public void ThreeArg10()
        {
            Formula f = new Formula("a + b + c + d", s => "x", s => true);
            Assert.AreEqual(4.0, f.Evaluate(s => (s == "x") ? 1 : 0), 1e-6);
        }

        [TestMethod]
        public void GetVars1()
        {
            Formula f = new Formula("0");
            var expected = new HashSet<string>();
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars2()
        {
            Formula f = new Formula("x");
            var expected = new HashSet<string>();
            expected.Add("x");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars3()
        {
            Formula f = new Formula("a * b - c + d / e * 2.5e6");
            var expected = new HashSet<string>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            expected.Add("e");
            var actual = f.GetVariables();
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void GetVars4()
        {
            Formula f = new Formula("a * a + b * c - d * d");
            var expected = new HashSet<string>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            var actual = f.GetVariables();
            Assert.AreEqual(4, actual.Count);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void GetVars5()
        {
            Formula f = new Formula("x+y", s => s.ToUpper(), s => true);
            var expected = new HashSet<string>();
            expected.Add("X");
            expected.Add("Y");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars6()
        {
            Formula f = new Formula("x+y+z", s => s + s, s => true);
            var expected = new HashSet<string>();
            expected.Add("xx");
            expected.Add("yy");
            expected.Add("zz");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void ToString1()
        {
            Formula f1 = new Formula("7");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(7.0, f2.Evaluate(s => 0), 1e-6);
        }

        [TestMethod]
        public void ToString2()
        {
            Formula f1 = new Formula("x");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(8.0, f2.Evaluate(s => (s == "x") ? 8 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString3()
        {
            Formula f1 = new Formula("x", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(8.0, f2.Evaluate(s => (s == "X") ? 8 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString4()
        {
            Formula f1 = new Formula("a+b*(c-15)/2");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsLower(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString5()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => s, s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsLower(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString6()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsUpper(s[0]) ? 16 : 0), 1e-6);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }

        public string NormalizerDoesNothing(string s)
        {
            return s;
        }

        public string NormalizerAllCaps(string s)
        {
            return s.ToUpper();
        }

        public bool ValidatorDoesNothing(string s)
        {
            return true;
        }

        public bool ValidatorAllStringsAreCaps(string s)
        {
            char[] letters = s.ToCharArray();

            foreach(char letter in letters)
            {
                if (char.IsLower(letter))
                    return false;
            }

            return true;
        }

        public bool ValidatorAllStringsAreLower(string s)
        {
            char[] letters = s.ToCharArray();

            foreach (char letter in letters)
            {
                if (char.IsUpper(letter))
                    return false;
            }

            return true;
        }
    }
}
