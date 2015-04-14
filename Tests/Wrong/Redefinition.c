struct Type { double y; } a;
struct Type { double y; } b;

typedef struct Type A;
typedef int A;

int Shadowing(int x)
{
    int val;
    int val;
    if (x > 0)
    {
        int val = 3;
    }
    else
    {
        val = 2;
    }
    return val;
}
