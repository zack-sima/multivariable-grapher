using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class EquationParser : MonoBehaviour {
    public static List<string> operationSymbols = new List<string>() { "+", "-", "*", "/", "^", "$" }; //$=trig/other function
    public static List<string> allSymbols = new List<string>() { "+", "-", "*", "/", "^", "$", "(", ")", "x", "y", "z", "e", "π" };
    public static List<string> functionSymbols = new List<string>() { "sin", "cos", "tan", "ln", "abs", "asin", "arcsin", "acos", "arccos", "atan", "arctan" };

    void Start() {
        string inputString = "e^x+e^y";
        float x = 1, y = 1, z = 1;

        List<string> expression = ConvertStringToListedEquation(inputString);

        Operator model = new Operator(expression);
        print(model.Evaluate(x, y, z));
    }
    public static List<string> ConvertStringToListedEquation(string inputString) {
        int numStartIndex = 0;
        int functionStartIndex = 0;
        int index = 0;

        List<string> exp = new List<string>();
        foreach (char c in inputString) {
            if (allSymbols.Contains(c.ToString())) {
                exp.Add(c.ToString());
                numStartIndex = index + 1;
                functionStartIndex = index + 1;
            } else if (functionSymbols.Contains(c.ToString())) {
                if (inputString[index + 1] == '$') {
                    //add function
                    exp.Add(inputString.Substring(functionStartIndex, index + 1 - functionStartIndex));
                }
                numStartIndex = index + 1;
                functionStartIndex = index + 1;
            } else { //should be a number
                functionStartIndex = index + 1;
                if (index == inputString.Length - 1 || allSymbols.Contains(inputString[index + 1].ToString())) {
                    //substring is different from python
                    exp.Add(inputString.Substring(numStartIndex, index + 1 - numStartIndex));

                    numStartIndex = index + 1;
                }
            }
            index++;
        }
        print(string.Join("", exp));
        return CorrectExpression(exp);
    }

    public static List<string> StripEdgeParenthesis(List<string> s) {
        while (s[0] == "(" && s[^1] == ")") {
            int depth = 0, index = 0;
            foreach (string i in s) {
                if (i == "(")
                    depth++;
                else if (i == ")" && index != s.Count - 1)
                    depth--;
                //depth should not be 0 if stripped
                if (depth <= 0)
                    break;
                index++;
            }
            if (depth <= 0) {
                break;
            }
            s = new List<string>(s.Skip(1).Take(s.Count - 2));
        }
        return s;
    }
    public class Operator {
        public string value;

        public bool isFunction;

        public string operation;
        public Operator left;
        public Operator right;

        public Operator(List<string> expression) {
            int netParenthesis = 0;
            int index = 0;
            if (expression.Count == 1) {
                //a number
                this.value = expression[0];

                //function
                if (functionSymbols.Contains(expression[0])) {
                    this.isFunction = true; //prevents evaluate() from getting called
                }
            } else {
                foreach (string s in expression) {
                    if (s == "(") {
                        netParenthesis++;
                    } else if (s == ")") {
                        netParenthesis--;
                    } else if (netParenthesis == 0 && operationSymbols.Contains(s)) {
                        //recursive evaluation

                        //negatives
                        if (s == "-" && index == 0) {
                            this.left = new Operator(new List<string>() { "0" });
                        } else {
                            this.left = new Operator(StripEdgeParenthesis(new List<string>(expression.Take(index))));
                        }
                        this.right = new Operator(StripEdgeParenthesis(new List<string>(expression.Skip(index + 1))));
                        this.operation = s;
                    }
                    index++;
                }
            }
            //print(string.Join(", ", expression));
        }
        public float Evaluate(float x, float y, float z) {
            //if it is a constant value
            if (this.value != "") {
                if (float.TryParse(this.value, out float val)) {
                    return val;
                }
                switch (this.value) {
                    case "x":
                        return x;
                    case "y":
                        return y;
                    case "z":
                        return z;
                    case "e":
                        return 2.71828f;
                    case "π":
                        return Mathf.PI;
                }
            }

            //evaluate operations
            if (this.left == null)
                print("null");
            if (this.right == null)
                print("null");

            float right = this.right.Evaluate(x, y, z);
            if (this.left.isFunction) {
                //evaluate function
                switch (this.left.value) {
                    case "sin":
                        return Mathf.Sin(right);
                    case "cos":
                        return Mathf.Cos(right);
                    case "tan":
                        return Mathf.Tan(right);
                    case "ln":
                        return Mathf.Log(right);
                    case "abs":
                        return Mathf.Abs(right);
                    case "asin":
                    case "arcsin":
                        return Mathf.Asin(right);
                    case "acos":
                    case "arccos":
                        return Mathf.Acos(right);
                    case "atan":
                    case "arctan":
                        return Mathf.Atan(right);
                    default:
                        Debug.LogWarning("function not known: " + this.left.value);
                        return 0;
                }
            } else {
                float left = this.left.Evaluate(x, y, z);

                //operations
                switch (this.operation) {
                    case "+":
                        return left + right;
                    case "-":
                        return left - right;
                    case "*":
                        return left * right;
                    case "/":
                        return left / right;
                    case "^":
                        return Mathf.Pow(left, right);
                    default:
                        Debug.LogWarning("operation not known: " + this.operation);
                        return 0;
                }
            }
        }
    }
    static List<string> CorrectExpression(List<string> exp) {
        //add multiplication sign to two adjacent non-symbols
        int index = 0;
        while (index < exp.Count - 1) {
            List<string> leftSymbols = new List<string>() { "+", "-", "*", "/", "^", "(", "$" };
            List<string> rightSymbols = new List<string>() { "+", "-", "*", "/", "^", ")", "$" };

            if (functionSymbols.Contains(exp[index]) && exp[index + 1] == "(") {
                //functions
                exp = new List<string>(exp.Take(index + 1).Concat(new List<string>() { "$" }).Concat(exp.Skip(index + 1)));
            } else if (!leftSymbols.Contains(exp[index]) && !rightSymbols.Contains(exp[index + 1])) {
                exp = new List<string>(exp.Take(index + 1).Concat(new List<string>() { "*" }).Concat(exp.Skip(index + 1)));
            }
            index++;
        }
        // add parenthesis with pemdas ordering

        // only correct expressions in current depth; when a parenthesis is seen at depth 0 mark it down,
        // add a depth, and when the closing parenthesis is seen for 0 depth call recursively to solve the expression inside
        index = 0;
        int depth = 0;
        List<int> operationsSeen = new List<int>();

        while (index < exp.Count) {
            if (exp[index] == "(") {
                depth++;
            } else if (exp[index] == ")") {
                depth--;
            }
            //reset index to 0 if a pair of parenthesis is added anywhere
            if (depth == 0 && operationSymbols.Contains(exp[index])) {
                operationsSeen.Add(index);
            }

            if (index == exp.Count - 1 && operationsSeen.Count > 1) {
                //compare pemdas and find the lowest one
                bool parenthesisAdded = false;

                List<int> reverseOperations = new List<int>(operationsSeen);
                reverseOperations.Reverse();

                //+-
                if (!parenthesisAdded) {
                    foreach (int i in reverseOperations) {
                        List<string> signs = new List<string>() {"+", "-"};
                        if (signs.Contains(exp[i])) {
                            List<string> originalExp = new List<string>(exp);
                            exp = new List<string>(new List<string>() { "(" }.Concat(originalExp.Take(i)).Concat(new List<string>() { ")" }).
                                Concat(new List<string>() { originalExp[i] }).Concat(new List<string>() { "(" }).
                                Concat(originalExp.Skip(i + 1)).Concat(new List<string>() { ")" }));
                            parenthesisAdded = true;
                            break;
                        }
                    }
                }
                //*/
                if (!parenthesisAdded) {
                    foreach (int i in reverseOperations) {
                        List<string> signs = new List<string>() { "*", "/" };
                        if (signs.Contains(exp[i])) {
                            List<string> originalExp = new List<string>(exp);
                            exp = new List<string>(new List<string>() { "(" }.Concat(originalExp.Take(i)).Concat(new List<string>() { ")" }).
                                Concat(new List<string>() { originalExp[i] }).Concat(new List<string>() { "(" }).
                                Concat(originalExp.Skip(i + 1)).Concat(new List<string>() { ")" }));
                            parenthesisAdded = true;
                            break;
                        }
                    }
                }
                //^
                if (!parenthesisAdded) {
                    foreach (int i in reverseOperations) {
                        List<string> signs = new List<string>() { "^" };
                        if (signs.Contains(exp[i])) {
                            List<string> originalExp = new List<string>(exp);
                            exp = new List<string>(new List<string>() { "(" }.Concat(originalExp.Take(i)).Concat(new List<string>() { ")" }).
                                Concat(new List<string>() { originalExp[i] }).Concat(new List<string>() { "(" }).
                                Concat(originalExp.Skip(i + 1)).Concat(new List<string>() { ")" }));
                            parenthesisAdded = true;
                            break;
                        }
                    }
                }
                //functions don't need to be separated since they're assumed to have brackets

                //reset index and start over again
                index = 0;
                depth = 0;
                operationsSeen = new List<int>();

                continue;
            }
            index++;
        }
        //only start recursive stuff after every parenthesis has been fixed
        index = 0;
        depth = 0;
        int startingParenthesisIndex = -1;
        while (index < exp.Count) {
            if (exp[index] == "(") {
                if (depth == 0) {
                    startingParenthesisIndex = index;
                }
                depth++;
            }
            if (exp[index] == ")") {
                depth -= 1;
                if (depth == 0) {
                    //add the recursive definition to the list
                    int reversedIndex = exp.Count - index;
                    exp = new List<string>(exp.Take(startingParenthesisIndex + 1).Concat(CorrectExpression(new List<string>(exp.Skip(startingParenthesisIndex + 1).
                        Take(index - startingParenthesisIndex - 1)))).Concat(exp.Skip(index)));
                    index = exp.Count - reversedIndex;
                }
            }
            index++;
        }
        return exp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
