        //static void test3()
        //{
        //    var bound = 999999;
        //    var primes = GetPrimes(bound);
        //    int i, j, k, l, m;
        //    var len = 6;
        //    var target = 8;
        //    var no_more = 10 - target;
        //    var count = 0;
        //    var count2 = 0;
        //    var from = (int)Math.Pow(10, len - 1) + 1;
        //    var to = (int)Math.Pow(10, len) - 1;
        //    var master = new int[len];
        //    var worker = new int[len];
        //    var num = 0;

        //    for (i = from; i < to; i += 2)
        //    {
        //        if (primes[i] == false)
        //        {
        //            num_to_int_array(i, master, len);
        //            //if (master.Count(n => n == 0) < 2 || master.Count(n => n == 1) < 2 || master.Count(n => n == 3) < 2)
        //            //    continue;
        //            //test 9aa123, 9a1a23, 9a12a3, 91aa23, 91a2a3, 912aa3
        //            for (j = 0; j < len - 3; j++)
        //            {
        //                for (k = j + 1; k < len - 2; k++)
        //                {
        //                    for (l = k + 1; l < len - 1; l++)
        //                    {

        //                        copy_int_arrays(master, worker, len);
        //                        count = 0;
        //                        count2 = 0;
        //                        for (m = 0; m < 10; m++)
        //                        {
        //                            worker[j] = m;
        //                            worker[k] = m;
        //                            worker[l] = m;
        //                            num = int_array_to_num(worker, len);
        //                            if (primes[num] == false)
        //                            {
        //                                count++;
        //                                //primes[num] = true;
        //                            }
        //                            else
        //                                count2++;

        //                            if (count2 > no_more)
        //                                break;
        //                        }
        //                        if (count == target)
        //                        {
        //                            Console.WriteLine(i);
        //                            //return;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //}