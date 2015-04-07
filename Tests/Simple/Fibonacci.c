
int fibonacci(int n)
{
    if (n == 0) return 0;
    else if (n == 1) return 1;
    else
    {
        int first = fibonacci(n - 2);
        int second = fibonacci(n - 1);
        return first + second;
    }
}
