
struct A b;

struct A
{
    int x, y;
    void *z;
};

int Test(void)
{
    b.x = 5;
    b.y = 6;
    char local = 'a';
    b.z = (void*)&local;
    return b.x;
};
