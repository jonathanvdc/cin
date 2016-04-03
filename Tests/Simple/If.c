int cond(int x)
{
    return x;
}

int f()
{
    int x = 5;
    if (cond(x))
        x += 1;
    else
        x += 2;
    return x;
}
