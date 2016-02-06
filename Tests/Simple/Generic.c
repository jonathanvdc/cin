
struct A { int x; };

int Square(int Value)
{
    return Value * Value;
}

struct A SquareA(struct A Value)
{
    struct A result;
    result.x = Square(Value.x);
    return result;
}

double SquareDouble(double Value)
{
    return Value * Value;
}

#define Sqr(x) _Generic(x, int: Square, \
                           struct A: SquareA, \
                           default: SquareDouble)(x)

struct A Test(void)
{
    struct A val;
    val.x = 5;
    return Sqr(val);
}
