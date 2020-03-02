#include <iostream>
#include <fstream>
#include <string>

using namespace std;

class Parser {

public:

	int parse(char* file) {

		cout << "Parsing from file: " << file << endl;

		ifstream f_in(file, ios::in);

		string loc_start, loc_end;
		int x_dist, y_dist;

		while (f_in >> loc_start >> loc_end >> x_dist >> y_dist)
		{

			if (x_dist < 0) {
				cout << "Cannot drive from " << loc_start << " to " << loc_end
					<< ", however, there is a straight line distance of " << y_dist << "km." << endl;
			}
			else
			{

				cout << "Travel from " << loc_start << " to " << loc_end
					<< " with a straight line distane of " << y_dist 
					<< "km or an actual distance of " << x_dist
					<< "km." << endl;

			}
		}

		f_in.close();


		return 0;
	}

};

int main(int argc, char* argv[])
{

   char* _file = argv[1];

   Parser distanceParser;

   distanceParser.parse(_file);
   
    
}