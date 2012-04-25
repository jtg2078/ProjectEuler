#include <iostream>
#include <string>
#include <fstream>
using namespace std;

int using_linq_and_algorithm_from_problem_18();
int main()  
{
	cout << "The maximum total from top to bottom in triangle.txt: \n";
	int answer = using_linq_and_algorithm_from_problem_18();
	cout << "using_linq_and_algorithm_from_problem_18 way: " << answer;

	cout << "\nPress any key to exit";
	string input = "";
	getline(cin, input);
	return 0; 
}

int using_linq_and_algorithm_from_problem_18()
{
	short triangle[100][100];
	std::ifstream src;
	src.open("triangle.txt");
	if (src.is_open() == false)
	{
		cout << "\nFile(triangle.txt) is not found!";
		return 0;
	}
	for (int row = 0; row < 100; row++)
	{
		for (int col = 0; col <= row; col++)
		{
			src >> triangle[row][col];
		}
	}
	src.close();
	for (int row = 98; row >= 0; row--)
	{
		for (int col = 0; col <= row; col++)
		{
			triangle[row][col] += max(triangle[row + 1][col], triangle[row + 1][col + 1]);
		}
	}
	return triangle[0][0];
}