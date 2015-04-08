
int Square(int Value)
{
    return Value * Value;
}

int SquareDouble(double Value)
{
    return Value * Value;
}

int Test(void)
{
    int val = 5;
    return _Generic(val, double: SquareDouble(val),
                         int: Square(val));
}
