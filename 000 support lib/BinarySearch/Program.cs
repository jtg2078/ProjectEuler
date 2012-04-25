using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinarySearch
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        static int binary_search(int[] list, int target)
        {
            if (list == null || list.Length <= 0)
                return -1;
            var len = list.Length;
            var start = 0;
            var pivot = 0;
            var end = len = 1;

            while (start >= 0 && start <= len && end >= 0 && end <= len)
            {
                if (list[start] == target)
                    return start;

                if (list[end] == target)
                    return end;

                if (start == end || end - start == 1)
                    return -1;

                pivot = start + (start - end) / 2;

                if (list[pivot] == target)
                    return pivot;
                else if (list[pivot] > target)
                    end = pivot - 1;
                else if (list[pivot] < target)
                    start = pivot + 1;
            }
            return -1;
        }

        static int binary_search_2(int[] list, int key)
        {
            if (list == null || list.Length <= 0)
                return -1;
            var low = 0;
            var high = list.Length - 1;
            while (low <= high)
            {
                var mid = low + (high - low) / 2;
                var mid_value = list[mid];
                if (mid_value < key)
                    low = mid + 1;
                else if (mid_value > key)
                    high = mid - 1;
                else
                    return mid;
            }
            return -1;
        }

        static void AssertLoader(Func<int[], int, int> test_method, int[] test_data, int search_target, int expected_result)
        {
            int actual_result = -2;

            try
            {
                actual_result = test_method(test_data, search_target);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed. Reason: Exception. Message: " + ex.Message);
                return;
            }
            if (actual_result == expected_result)
                Console.WriteLine(string.Format("Passed. Expected:{0}. Actual:{1}", expected_result, actual_result));
            else
                Console.WriteLine(string.Format("Failed. Expected:{0}. Actual:{1}", expected_result, actual_result));
        }
    }
}
