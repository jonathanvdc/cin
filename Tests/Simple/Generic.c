
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

int Test(void)
{
    struct A val;
    val.x = 5;
    return _Generic(val, int: Square(val),
                         struct A: SquareA(val),
                         default: SquareDouble(val));
}
