namespace MathFuncConsole.MathObjects.Helper {
    internal static class LinearAlgberaHelper {
        public static double[] LuSolver(double[,] A, double[] b) {
            //function x=LU_fenjieqiuxianxingfangcheng(A,b)  
            //n=size(A,1);
            var n = A.GetLength(0);

            //for j=1:n
            //    u(1,j)=A(1,j);
            //end
            var u = new double[n, n];
            for (var i = 0; i < n; i++)
                u[0, i] = A[0, i];

            //for i=2:n
            //    l(i,1)=A(i,1)/u(1,1);
            //end
            var l = new double[n, n];
            for (var i = 1; i < n; i++)
                l[i, 0] = A[i, 0] / u[0, 0];

            //for i=2:(n-1)
            for (var i = 1; i < n - 1; i++) {

                //    clear SUM1
                //    SUM1=0;
                var sum1 = 0D;

                //    for k=1:(i-1)
                //    SUM1=SUM1+l(i,k)*u(k,i);
                //    end 
                for (var k = 0; k < i; k++)
                    sum1 += l[i, k] * u[k, i];

                //    u(i,i)=A(i,i)-SUM1;
                u[i, i] = A[i, i] - sum1;

                //    for j=(i+1):n
                for (var j = i + 1; j < n; j++) {
                    //        clear SUM2
                    //        SUM2=0;
                    var sum2 = 0D;

                    //        for k=1:(i-1)
                    //        SUM2=SUM2+l(i,k)*u(k,j);
                    //        end
                    for (var k = 0; k < i; k++)
                        sum2 += l[i, k] * u[k, j];

                    //        u(i,j)=A(i,j)-SUM2;
                    u[i, j] = A[i, j] - sum2;

                    //        clear SUM3
                    //        SUM3=0;
                    var sum3 = 0D;

                    //        for k=1:(i-1)
                    //        SUM3=SUM3+l(j,k)*u(k,i);
                    //        end
                    for (var k = 0; k < i; k++)
                        sum3 += l[j, k] * u[k, i];

                    //        l(j,i)=(A(j,i)-SUM3)/u(i,i);
                    l[j, i] = (A[j, i] - sum3) / u[i, i];
                }
            }

            //clear SUM4
            //        SUM4=0;
            var sum4 = 0D;

            //        for k=1:(n-1)
            //        SUM4=SUM4+l(n,k)*u(k,n);
            //        end
            for (var k = 0; k < n - 1; k++)
                sum4 += l[n - 1, k] * u[k, n - 1];

            //u(n,n)=A(n,n)-SUM4;
            u[n - 1, n - 1] = A[n - 1, n - 1] - sum4;

            //for i=1:n
            //    l(i,i)=1;
            //end
            for (var i = 0; i < n; i++)
                l[i, i] = 1;

            //y(1,1)=b(1,1);
            var y = new double[n];

            //for i=2:n
            for (var i = 1; i < n; i++) {
                //    clear SUM5
                //    SUM5=0;
                var sum5 = 0D;

                //    for j=1:(i-1)
                //        SUM5=SUM5+l(i,j)*y(j,1);
                //    end
                for (var j = 0; j < i; j++)
                    sum5 += l[i, j] * y[j];

                //    y(i,1)=b(i,1)-SUM5;
                y[i] = b[i] - sum5;
            }

            var x = new double[n];

            //x(n,1)=y(n,1)/u(n,n);%求解Ux=y
            x[n - 1] = y[n - 1] / u[n - 1, n - 1];

            //for i=(n-1):(-1):1
            for (var i = n - 2; i >= 0; i--) {
                //    clear SUM5
                //    SUM5=0;
                var sum5 = 0D;

                //    for j=(i+1):n
                //        SUM5=SUM5+u(i,j)*x(j,1);
                //    end
                for (var j = i + 1; j < n; j++)
                    sum5 += u[i, j] * x[j];

                //    x(i,1)=(y(i,1)-SUM5)/u(i,i);
                x[i] = (y[i] - sum5) / u[i, i];
            }
            return x;
        }

        public static double[] CatchSolver(double[,] A, double[] d) {
            //function x=ZhuiGanFa(A,d) 
            //n=size(A,1);
            //u(1)=A(1,1);
            //y(1)=d(1,1);
            var n = A.GetLength(0);
            var u = new double[n];
            var y = new double[n];
            u[0] = A[0, 0];
            y[0] = d[0];

            //for k=2:n
            for (var k = 1; k < n; k++) {
                //    clear l1
                //    l1=A(k,k-1)/u(k-1);
                var l1 = A[k, k - 1] / u[k - 1];

                //    u(k)=A(k,k)-l1*A(k-1,k);
                //    y(k)=d(k,1)-l1*y(k-1);
                u[k] = A[k, k] - l1 * A[k - 1, k];
                y[k] = d[k] - l1 * y[k - 1];

                //end
            }

            //x(n,1)=y(n)/u(n);
            var x = new double[n];
            x[n - 1] = y[n - 1] / u[n - 1];

            //for m=(n-1):(-1):1
            //    x(m,1)=(y(m)-A(m,m+1)*x(m+1,1))/u(m);
            //end
            for (var m = n - 2; m >= 0; m--) {
                x[m] = (y[m] - A[m, m + 1] * x[m + 1]) / u[m];
            }

            return x;
        }
    }
}