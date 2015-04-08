
int Test(void)
{
    struct { int x; } a;
    struct Type { int y; } b;
    a.x = 5;
    b.y = a.x;
    return b.y;
}

double Test2(void)
{
    struct { double x; } a;
    struct Type { double y; } b;
    a.x = 5;
    b.y = a.x;
    return b.y;
}
