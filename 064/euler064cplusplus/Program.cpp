#include <iostream>
#include <math.h>
using namespace std;

int using_iterative_algorithm_to_calculate_continued_fraction_expansion();
int is_perfect_square(int n);
int main()  
{  
	cout << "The numbers of continued fractions for N <= 10000 have an odd period: \n";
	int ans = using_iterative_algorithm_to_calculate_continued_fraction_expansion();
	cout <<"using_iterative_algorithm_to_calculate_continued_fraction_expansion way: "<< ans;

	cout <<"\nPress any key to exit";
	double d;
	cin>>d;
	return 0;  
}

int using_iterative_algorithm_to_calculate_continued_fraction_expansion()
{
    int result = 0;
    for (double i = 2; i <= 10000; i++)
    {
        if (is_perfect_square(i) == false) // perfect squares are skipped
        {
            int a0 = (int)sqrt(i);
            int m = 0;
            int d = 1;
            int a = a0;
            int counter = -1; // since the 1st iteration is not part of number chains
            int m1 = INT_MIN; // reset m1 and d1
            int d1 = INT_MIN;
            while (true)
            {
                counter++;
                
                if (counter == 1)
                {
                    m1 = m; // so these two are marked as head of the period
                    d1 = d;
                }

                m = d * a - m;
                d = (i - m * m) / d;
                a = (a0 + m) / d;

                if (m == m1 && d == d1) // so end of period is found
                    break;
            }
            if ((counter & 1) != 0)
                result++;
        }
    }
    return result;
}

//http://stackoverflow.com/questions/295579/fastest-way-to-determine-if-an-integers-square-root-is-an-integer/295678#295678
int is_perfect_square(int n)
{
    int h = n & 0xF;  // h is the last hex "digit"
    if (h > 9)
        return 0;
    // Use lazy evaluation to jump out of the if statement as soon as possible
    if (h != 2 && h != 3 && h != 5 && h != 6 && h != 7 && h != 8)
    {
        int t = (int) floor( sqrt((double) n) + 0.5 );
        return t*t == n;
    }
    return 0;
}
