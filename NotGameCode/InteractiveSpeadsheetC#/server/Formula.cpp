/// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

///<author>Paul C Carlson</author>

#include "../include/Formula.h"
#include <regex>
#include <string>
#include <stack>
#include <vector>
#include <functional>
#include <tr1/regex>
#include <iterator>
#include <stdexcept>
#include <stdlib.h>
#include <limits>
#include <cstring>


namespace cs3505{

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
class FormulaError
{
public:

	FormulaError(){};
      /// <summary>
      /// Constructs a FormulaFormatException containing the explanatory message.
      /// </summary>
    FormulaError(std::string reason)
    {
    	this->Reason = reason;
    }

      std::string Reason;
};






    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision
    /// floating-point syntax; variables that consist of a letter or underscore followed by
    /// zero or more letters, underscores, or digits; parentheses; and the four operator
    /// symbols +, -, *, and /.
    ///
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and "y"; "x23" is a single variable;
    /// and "x 23" consists of a variable "x" and a number "23".
    ///
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    //class Formula
    //{
        // A list of the allowed operators
        //private List<String> validOps = new List<string>() { "+", "-", "*", "/" };

        // A list of the tokens once the formula object is created, variable tokens are normalized

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.
        /// </summary>
        Formula::Formula(std::string formula){
	  *this = Formula(formula, [](std::string s) { return s;} ,[] (std::string s) {return true;});
        }

        Formula::~Formula(){

          delete &tokens;
      	  delete &values;
      	  delete &operators;

      	  delete &operatorList;

      	  delete this;
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.
        ///
        /// If the formula contains a variable v such that normalize(v) is not a legal variable,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        ///
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        Formula::Formula(std::string formula, std::string normalize (std::string), bool isValid (std::string))
        {

	  tokens = *(new std::vector<std::string>());
	  values = new std::stack<double>();
	  operators = new std::stack<std::string>();

            // Parse the formula and put the tokens into a list for later use
            std::list<std::string> inputFormula = GetTokens(formula);

            // Check if the token is a number, variable or operator before trying to add to the token list
            for(std::list<std::string>::iterator token = inputFormula.begin(); token != inputFormula.end(); ++token)
            {

                //eclipse bitches without an inbetween.
                std::string tokenString = *token;
                const char * tokenChar = tokenString.c_str();

                char* pEnd;

                double number = strtod(tokenChar, &pEnd);

                if (IsValidOperator(tokenString) || tokenString == ")" || tokenString == "(")
                {
                    tokens.push_back(tokenString);
                }
                else if (number != 0.0 || tokenChar == "0.0")
                {
                    tokens.push_back(tokenString);
                }
                else if (IsValidVariable(tokenString, normalize, isValid))
                {
                    tokens.push_back(normalize(tokenString));
                }
                else
                {
                    // The token is not an operator, a number or a variable so throw an exception
                    throw new std::invalid_argument(*token + " is not a valid token");
                }
            }


            // Check that formula matches the following criteria:
            //  contains at least one token
            if (tokens.size() < 1)
            {
                throw new std::invalid_argument("The provided formula has no tokens in it.");
            }

            //  the first token is a variable or a number or an opening paren
            if (*(tokens.begin()) == ")" || (IsValidOperator(*(tokens.begin()))&& *(tokens.end()) != "("))
            {
                throw new std::invalid_argument(*tokens.begin() + " is not a valid first token.");
            }
            //  the last token is a variable a number or a closing paren
            if (*(tokens.end()) == "(" || (IsValidOperator(*(tokens.end())) && *(tokens.end()) != ")"))
            {
                throw new std::invalid_argument(*(tokens.end()) + " is not a valid ending token.");
            }

            //  opening and closing parens match reading left to right
            int balance = 0;
            for (int i = 0; i < tokens.size(); i++)
            {
                if (tokens[i] == "(") { balance++; }
                else if (tokens[i] == ")") { balance--; }

                if (balance < 0)
                {
                    throw new std::invalid_argument("Reading right to left, there is an unmatched closing parenthesis");
                }
            }
            if (balance != 0)
            {
                throw new std::invalid_argument("There was an unmatched opening parenthesis.");
            }

            for (int j = 0; j < tokens.size() - 1; j++)
            {
                //  the token following an open paren is a variable or a token or an open paren
                if (tokens[j] == "(")
                {
                    if(tokens[j + 1] == ")" || IsValidOperator(tokens[j + 1]))
                    {
                        throw new std::invalid_argument("The token following an opening parenthesis was not a number, variable or another opening parenthesis");
                    }
                }
                //  the token following a number or variable or closing paren is an operator or closing paren
                else if (tokens[j] == ")" || !IsValidOperator(tokens[j]))
                {
                    if (!(tokens[j + 1] == ")" || IsValidOperator(tokens[j + 1])))
                    {
                        throw new std::invalid_argument("The token following a number, variable or closing parenthesis was not an operator or another closing parenthesis.");
                    }
                }
                // make sure operators don't follow operators
                else if (IsValidOperator(tokens[j]))
                {
                    if(IsValidOperator(tokens[j+1]) || tokens[j+1] == ")")
                    {
                        throw new std::invalid_argument("The token following an operator was either another operator or a close parenthesis.");
                    }
                }
            }
        }

	/// <summary>
	/// Checks if the provided string is a valid operator.
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
        bool Formula::IsValidOperator(std::string s){
      	  operatorList = { "+", "-", "*", "/" };
        	for(int i = 0; i < 4; i++){
        		if (this->operatorList[i] == s){
        			return true;
        		}
        	}

        	return false;
        }


        /// <summary>
        /// Checks if a provided string is a valid varible name
        /// </summary>
        /// <param name="s"></param>
        /// <param name="normalize"></param>
        /// <param name="isValid"></param>
        /// <returns></returns>
        bool Formula::IsValidVariable(std::string s, std::string normalize (std::string), bool isValid (std::string))
        {
            s = normalize(s);

	    bool valid = true;
	    std::string::iterator t = s.begin();

	    char ch = *t;
	    
	    //If the first char isn't a letter invalidate and jump
	    if(ch > 'Z' || ch < 'A'){
	      valid = false;
	      //If the first char is a letter continue checking
	    } else {
	      t++;
	      ch = *t;
	      //If the next char isn't a digit then invalidate and jump
	      if(!isdigit(ch)){
		valid = false;
		//If the second char is a digit then check the next is a digit or end
	      } else {
		t++;
		ch = *t;
		//If the last char isn't a digit and isn't the end then invalidate and jump
		if(!isdigit(ch) && t != s.end()){
		  valid = false;
		}
	      }
	    }

            if (!valid)
            {
                throw new std::invalid_argument(s + " does not follow the basic pattern for a valid variable name.");
            }

            return true;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
        /// the constructor.)
        ///
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
        /// in a string to upper case:
        ///
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        ///
        /// Given a variable symbol as its parameter, lookup returns the variable's value
        /// (if it has one) or throws an ArgumentException (otherwise).
        ///
        /// If no undefined variables or divisions by zero are encountered when evaluating
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        double Formula::Evaluate(int Lookup (std::string))
        {
            // The stacks we will use when evaluating the function
      	  this->values = new std::stack<double>;
      	  this->operators = new std::stack<std::string>;

            // Start evaluating the tokens, make sure to catch any exceptions so we
            // can return a FormulaError instead!
      	  for (std::vector<std::string>::iterator t = this->tokens.begin() ; t != this->tokens.end(); ++t)
      	  {
	    std::string tstring = *t;

                
	    try
                {
                    // Handle Operands

		const char * tchar = tstring.c_str();
		char* pEnd;

                    double x = strtod(tchar, &pEnd);
                    // If token is an int
                    if (x != 0.0 || tstring == "0.0")
                    {
                        UseValues(x, *values, *operators);
                    }
                    // If token is a variable
                    else if (IsVariable(tstring))
                    {
                        UseValues(Lookup(tstring), *values, *operators);
                    }
                    // Handle Operators
                    else if (IsOperator(tstring))
                    {
                        UseOperator(tstring, *values, *operators);
                    }
                }
                catch (std::invalid_argument & e)
                {
                    //return new CS3505::FormulaError(e.what);
			return -std::numeric_limits<double>::max();

                }
            }

            // Handle remaining operators
            if (this->operators->size() != 0)
            {
                if (this->operators->size() > 1 || values->size() != 2)
                {
                    //return new CS3505::FormulaError("Not all operators were used when evaluating the formula.");
			return -std::numeric_limits<double>::max();
                }
                else
                {
                    AddOrSubtract(*values, *operators);
                }
            }
            // Handle excess variables
            else if (values->size() != 1)
            {
                //return new CS3505::FormulaError("Evaluating the formula did not result in exactly one value.");
		    return -std::numeric_limits<double>::max();

            }

            // If there are no errors, return the result of evaluating exp
            return values->top();
        }


        /// <summary>
        /// UseOperator is called when the token being evaluated is not an int, a variable, "+" or "-"
        /// but does match one of the possible operators in operatorList.
        /// </summary>
        /// <param name="t"></param>
        void Formula::UseOperator(std::string t, std::stack<double> &values, std::stack<std::string> &operators)
        {
            // Handle "*", "/" and "(" tokens
            if (t == "*" || t == "/" || t == "(")
            {
                operators.push(t);
            }

            // Handle "+" and "-" tokens
            else if (t == "+" || t == "-")
            {
                AddOrSubtract(values, operators);
                operators.push(t);
            }

            // Handle ")" tokens, two step process
            else if (t == ")")
            {
                if (operators.size() > 0 && (operators.top() == "+" || operators.top() == "-"))
                {
                    AddOrSubtract(values, operators);
                }

                if (operators.size() > 0 && operators.top() == "(") { operators.pop(); }

                // The stack may be empty at this point which is okay! But skip the following.
                if (operators.size() > 0 && (operators.top() == "*" || operators.top() == "/"))
                {
                    if (values.size() < 2)
                    {
                        throw new std::invalid_argument("Error while multiplying or dividing, 'Values' stack empty.");
                    }
                    std::string temp = operators.top();
                    double b = values.top();
                    double a = values.top();
                    if (temp == "*")
                    {
                        values.push(a * b);
                    }
                    else if (temp == "/")
                    {
                        if (b == 0.0)
                        {
                            throw new std::invalid_argument("There was a divied by 0 error");
                        }
                        values.push(a / b);
                    }
		    operators.pop();
		    values.pop();
		    values.pop();
                }
            }
        }

        /// <summary>
        /// AddOrSubtract is called when the top of the operator stack is a "+" or "-".
        /// </summary>
        void Formula::AddOrSubtract(std::stack<double> &values, std::stack<std::string> &operators)

        {
            if (operators.size() == 0) { return; }

            // Check the Operator stack and add or subtract
            else if (operators.top() == "+")
            {
                if (values.size() < 2)
                {
                    throw new std::invalid_argument("Not enough values while adding.");
                }
                else
                {
                    operators.pop();
                    double b = values.top();
                    double a = values.top();
                    values.push(a + b);
		    values.pop();
		    values.pop();
                }
            }

            else if (operators.top() == "-")
            {
                if (values.size() < 2)
                {
                    throw new std::invalid_argument("Not enough values while subtracting.");
                }
                else
                {
                    operators.pop();
                    double b = values.top();
                    double a = values.top();
                    values.push(a - b);
		    values.pop();
		    values.pop();
                }
            }
        }


        /// <summary>
        /// IsOperator checks whether t is a member of operatorList.
        /// </summary>
        /// <param name="t"></param>
        /// <returns> true if t is an operator, false otherwise. </returns>
        bool Formula::IsOperator(std::string t)
        {
            return IsValidOperator(t) || t == "(" || t == ")";
        }


        /// <summary>
        /// Confirms that the token is a variable
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Formula::IsVariable(std::string t)
        {
	  const char * tokenChar = t.c_str();
            char* pEnd;

            double number = strtod(tokenChar, &pEnd);

            return (!IsOperator(t) && !(number != 0.0 || t == "0.0"));
        }

        /// <summary>
        /// UseValues is called when the token being evaluated is an int or a variable.
        /// The token is either operated or pushed to the value stack.
        /// </summary>
        /// <param name="b"></param>
        void Formula::UseValues(double b, std::stack<double> &values, std::stack<std::string> &operators)
        {
            if (operators.size() == 0) { values.push(b); }
            else if (operators.top() == "*")
            {
                if (values.size() == 0)
                {
                    throw new std::invalid_argument("Error while multiplying or dividing, 'Values' stack empty.");
                }
                operators.pop();
                double a = values.top();
                a = a * b;
		values.pop();
                values.push(a);
	       
            }
            else if (operators.top() == "/")
            {
                if (values.size() == 0)
                {
                    throw new std::invalid_argument("Error while multiplying or dividing, 'Values' stack empty.");
                }
                operators.pop();
                double a = values.top();
                // Check for divide by 0 errors
                if (b == 0.0)
                {
                    throw new std::invalid_argument("There was a divied by 0 error");
                }
                a = a / b;
		values.pop();
                values.push(a);
            }
            else { values.push(b); }
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this
        /// formula.  No normalization may appear more than once in the enumeration, even
        /// if it appears more than once in this Formula.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        std::list<std::string> Formula::GetVariables()
        {
	  std::list<std::string> variables;

	  for (std::vector<std::string>::iterator it = tokens.begin() ; it != tokens.end(); ++it)
            {
	      const char * tokenChar = (*it).c_str();
                char* pEnd;

                double number = strtod(tokenChar, &pEnd);

                if (!(IsOperator(tokenChar) || (number != 0.0 || *it == "0.0")))
                {
                    variables.push_back(tokenChar);
                    variables.unique(tokenChar);
                }
            }

            return variables;
        }


        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        std::string Formula::ToString() const
        {
            std::string formula = "";
            for (std::vector<std::string>::const_iterator it = tokens.begin() ; it != tokens.end(); ++it)
            {
                formula = formula + *it;
            }

            return formula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        ///
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        bool Formula::Equals(void* obj)
        {
            void* temp = this;

            if (temp == NULL && obj == NULL)
            {
                return true;
            }
            else if (obj == NULL)
            {
                return false;
            }

            //Try to convert, if unable then inequal
            try{
	      //Formula &f = dynamic_cast<Formula&>(obj);
            } catch (const std::bad_cast&  e){
            	return false;
            }

            //std::string f1 = this->ToString()(;
            //std::string f2 = f.ToString();

            //return f1 == f2;
	    //
	    return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        //bool Formula::operator ==(Formula f1, Formula f2)
        /// </summary>
        bool Formula::operator==(const Formula& rhs){ 
            // Cast f1 and f2 to object class to make sure == for object is being used
            //void* left = this;
            //void* right = rhs;
            //if (left == NULL && right == NULL)
            //{
            //    return true;
            //}
            //else if (left == NULL || right == NULL)
            //{
            //    return false;
            //}

	  //Formula fl;
	  //	Formula fr;
          //Try to convert, if unable then not formula
	  //try{
	  //	fl = (Formula) lhs;
	  //	fr = (Formula) rhs;
	  //} catch (std::exception& e){
	  //	return false;
	  //}

            // Get the string equivalent of f1 and f2
            std::string lHand = this->ToString();
            std::string  rHand = rhs.ToString();

            // Return whether the strings produced by f1 and f2 match
            return lHand == rHand;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
  bool Formula::operator!=(const Formula& rhs)
        {
            return !(*this == rhs);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        int Formula::GetHashCode()
        {
            // We will take advantage of the fact that String has a GetHashCode() method
	  //GetHashCode has no use, replaced with 5.
            std::string f = this->ToString();
            return 5;//boost::hash<std::string>{}(f);
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
  std::list<std::string> GetTokens(std::string formula)
        {
	  std::list<std::string> output;

	  for (std::string::iterator it = formula.begin() ; it != formula.end(); ++it){

	      std::string out = "";
	      char ch = *it;

	      //If the current char is a letter it is a var, followed by 1 or 2 numbers.
	      if(ch <= 'Z' && ch >= 'A'){
		//Take first char and second as these are guarenteed part of var
		out+=ch;
		it++;
		ch = *it;
		out+=ch;
		//Check if the next char is a number, this means its part of the var
		it++;
		ch = *it;
		if(isdigit(ch)){
		  out+= ch;
		  //If the next char isn't a digit don't add to out string and decrement the iterator
		} else {
		  it--;
		}
		output.push_back(out);
		continue;		
	      }

	      //If the char is a digit then add to output and check for connected digits
	      if(isdigit(ch)){
		out +=ch;
		it++;		
		ch = *it;
		//Iterate forward until the char we're on is not a digit AND not '.', since doubles can have a decimal
		while(isdigit(ch) || ch == '.'){
		  out+= ch;
		  it++;
		  ch = *it;
		}

		//Once done iterating we have our number, decrement the iterator and continue.
		output.push_back(out);
		it--;
		continue;
	      }
	      
	      //If the char is not a letter or a digit then it is an operator, add it alone to the string.
	      out+= ch;
	      output.push_back(out);
	      continue;

	    }

        return output;

        };

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    class FormulaFormatException : public std::exception
    {
    public:
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        explicit FormulaFormatException(const char* message):
        msg_(message)
        {}

        explicit FormulaFormatException(const std::string& message) : msg_(message){
        }

        virtual ~FormulaFormatException() throw (){}

    protected:

        std::string msg_;
    };



}
