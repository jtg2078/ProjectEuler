#include <iostream>
#include <vector>
#include <algorithm>
#include <string>
using namespace std;

vector<int> num_to_vector(int n);
vector<int> addition(const vector<int> &a, const vector<int> &b);
vector<int> multiply(const vector<int> &a, const vector<int> &b);
int using_convergents_and_formula_to_calculate_numerator();

int main()  
{
	cout << "The sum of digits in the numerator of the 100^(th) convergent of the continued fraction for e:\n";
	int answer = using_convergents_and_formula_to_calculate_numerator();
	cout <<"using_convergents_and_formula_to_calculate_numerator way: "<< answer;

	cout <<"\nPress any key to exit";
	string input = "";
	getline(cin, input);
	return 0; 
}

int using_convergents_and_formula_to_calculate_numerator()
{
    int every_third = 0;
    int a = 2;
    vector<int> p_2 = num_to_vector(0);
    vector<int> p_1 = num_to_vector(1);
    vector<int> p = addition(multiply(num_to_vector(a), p_1), p_2);

    int counter = -1;
    while (counter++ < 98)
    {
        if ((counter + 2) % 3 == 0)
        {
            every_third += 2;
            a = every_third;
        }
        else
            a = 1;
        
        p_2 = p_1;
        p_1 = p;
        p = addition(multiply(num_to_vector(a), p_1), p_2);
    }

	int p_len = p.size();
	int sum = 0;
	for (int i = 0; i < p_len; i++)
	{
		sum += p[i];
	}

	return sum;
}

vector<int> num_to_vector(int n)
{
    vector<int> result;
    do
    {
        result.push_back(n % 10);
    } while ((n /= 10) > 0);

    return result;
}

vector<int> addition(const vector<int> &a, const vector<int> &b)
{
	int a_len = a.size();
	int b_len = b.size();
	int len = max(a_len, b_len);
	vector<int> result;
	int x = 0;
	int y = 0;
	int sum = 0;
	int carry = 0;

	for (int i = 0; i < len; i++)
	{
		x = i < a_len ? a[i] : 0;
		y = i < b_len ? b[i] : 0;

		sum = x + y + carry;
		result.push_back(sum % 10);
		carry = sum / 10;
	}
	if(carry > 0)
		result.push_back(carry);
	
	return result;
}

vector<int> multiply(const vector<int> &a, const vector<int> &b)
{
    vector<int> result;
    int shift = 0; 
	int product = 0;
    int index = 0;
	int a_len = a.size();
	int b_len = b.size();
    for (int i = 0; i < a_len; i++)
    {
        shift = index;
        for (int j = 0; j < b_len; j++)
        {
            product = a[i] * b[j];
            if (shift >= result.size())
                result.push_back(0);
            result[shift] += product;
            shift++;
        }
        index++;
    }

    int carry = 0;
	int result_len = result.size();
    for (int i = 0; i < result_len; i++)
    {
        result[i] += carry;
        carry = result[i] / 10;
        result[i] = result[i] % 10;
    }
    if (carry > 0)
        result.push_back(carry);

    return result;
}