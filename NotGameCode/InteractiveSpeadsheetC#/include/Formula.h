
#include <functional>
#include <list>
#include <stack> 
#include <vector>
#include <string>

#ifndef FORMULA_H_
#define FORMULA_H_

namespace cs3505{

class Formula{
  public:
	  Formula(std::string formula);
	  virtual ~Formula();
      Formula(std::string formula, std::string normalize (std::string), bool isValid (std::string));
	double Evaluate(int variableEvaluator (std::string));
	  int f(double d);
      virtual std::string ToString() const; 
      std::list<std::string> GetVariables();
      virtual bool Equals(void* obj);
      bool operator==( const Formula& rhs);
      bool operator!=( const Formula& rhs);
      int GetHashCode();






  private:
	  void UseOperator(std::string t);
      std::list<std::string> GetTokens(std::string formula);
      void AddOrSubtract(std::stack<double> &values, std::stack<std::string> &operators);
      void UseValues(double b, std::stack<double> &values, std::stack<std::string> &operators);
	  bool IsOperator(std::string t);
	  bool IsVariable(std::string t);
      bool IsValidVariable(std::string s, std::string normalize (std::string), bool isValid (std::string));
      bool IsValidOperator(std::string s);
      void UseOperator(std::string t, std::stack<double> &values, std::stack<std::string> &operators);


      std::vector<std::string> tokens;
	  std::stack<double> *values;
	  std::stack<std::string> *operators;

	  // List of the possible operators
	  std::vector<std::string> operatorList;
  };

}

#endif /* FORMULA_H_ */
