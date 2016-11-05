using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CS422
{
    // notice that it's similar and easier to use BitConverter.DoubleToInt64Bits Method
    // use this unmanaged structure to attain the bits inside a double struct
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct MyUnion
    {
        [FieldOffset(0)] public double val;
        [FieldOffset(0)] public ulong bits;
    }

    public class BigNum
    {
        BigInteger m_base, m_exp; // base and exponential
        bool m_isUndefinded; // indicate whether it's undefined
        static int digits_resolution = 20; // resolution for how many minimal digits we need to display when we call Tostring

        // a direct constructor
        public BigNum(BigInteger mbase, BigInteger exp, bool isUndef)
        {
            m_base = mbase;
            m_exp = exp;
            m_isUndefinded = isUndef;
            Simplify();// do some siplification here
        }

        // constructor for building from a string
        public BigNum(string number)
        {
            number = number.ToLower(); // trans to lowercase first
            m_isUndefinded = false;

            // if invalid string, throw exception
            if (String.IsNullOrEmpty(number) || number.Contains(" "))
            {
                throw new ArgumentException("Not Valid String for BigInteger Class");
            }

            //initializing
            int sign = 1;
            int start = 0; // the start index of a digit '0' to '9'
            m_exp = new BigInteger(0);

            // check sign
            if (number[0] == '-')
            {
                sign = -1;
                start = 1;
            }

            m_base = new BigInteger(0);
            m_exp = new BigInteger(0);

            bool has_decimal_point = false; // whether we have a decimal point
            
            for (int i = 0 + start; i < number.Length; i++)
            {
                // if we met 'e', then we need to parse the remaining part to make it an integer exponential
                if (number[i] == 'e' && i < number.Length)
                {
                    string exponential_part = number.Substring(i + 1);
                    if (!string.IsNullOrEmpty(exponential_part))
                    {
                        int tmp_exp = 0;
                        if(int.TryParse(exponential_part, out tmp_exp))
                        {
                            m_exp += tmp_exp; // update the exponential
                            break;
                        }
                    }
                }

                // if it's an valid digit 0-9, we take it into our base
                if (number[i] <= '9' && number[i] >= '0')
                {
                    m_base = m_base * 10 + (number[i] - '0');
                    if (has_decimal_point) // if this digit is after decimal point, we need to decrease the exponential
                        m_exp--;
                } // if we first met a decimal point, set it to true, otherwise, throw exception, since no two decimal points allowed
                else if (number[i] == '.' && !has_decimal_point) 
                    has_decimal_point = true;
                else
                    throw new ArgumentException("Not Valid String for BigInteger Class");
            }
            
            // consider the sign
            m_base = sign * m_base;

            // simplify the bignum format
            Simplify();
        }

        // construct a BigNum and depends on useDoubleToString to translate using string or using internal double type formatting
        public BigNum(double value, bool useDoubleToString)
        {
            // initializing
            m_base = new BigInteger(0);
            m_exp = new BigInteger(0);

            // if isinfinity or isNaN, then set m_isUndefinded to be true
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                m_isUndefinded = true;
            }
            else if (useDoubleToString) // if we should use double to string, then call the other constructor with string as parameter
            {
                BigNum tmp = new BigNum(value.ToString());
                CopyFrom(tmp);
            }
            else
            {
                MyUnion tmp = new MyUnion(); // use union structure to get the sign, exponential, significant of a double
                tmp.val = value;
                ulong bits = tmp.bits; // this can be easily realized using BitConverter.DoubleToInt64Bits Method
                uint sign_bit = (uint)(bits >> 63);
                uint exp = (uint)((bits >> 52) & 0x7FF);
                ulong significant = bits & 0xFFFFFFFFFFFFF;
                
                // initialize a temp variable
                BigInteger tem_for_significant = new BigInteger(0);
                if (exp == 0)
                {
                    if ( significant == 0)// return if both exponential and significant are zero
                    {
                        m_isUndefinded = false;
                        return;
                    }
                }
                else if( exp == 0x7FF) // if exp is maximum, we set m_isUndefinded to be true
                { // actually this is already checked by double.IsInfinity(value) || double.IsNaN(value) before
                    m_isUndefinded = true;
                    return;
                }

                // digits indicates how much binary digits we increased in significant to make it a big integer
                int digits = 52;// do a simplification here to remove traling zeros within significant
                while(significant % 2 == 0 && significant != 0)
                {
                    significant = significant >> 1;
                    digits--;
                }
                if (significant == 0)
                    digits = 0;

                // the part below is due to definition of the fraction in double format
                // if exponential part is 0, we set tem_for_significant to twice of our significant
                // else, we need to add an one before the significant
                if (exp == 0)
                    tem_for_significant = 2 * new BigInteger(significant);
                else
                    tem_for_significant = new BigInteger(significant | (ulong) 1 << digits);

                // if exponential is bigger than the digits which we increased in significant, 
                // we multiply this significant by 2 to the power of this number
                if(exp > 1023 + digits)
                {
                    tem_for_significant *= BigInteger.Pow( 2, (int)exp - 1023 - digits);
                }
                else
                { // else, we need to use the logic of any number multiplied by 10^x * 10^(-x) is equal to itself
                    // then we get 10^(some digits) / (2^some digits), we get 5^(some digits) 
                    for (int i = 0; i < 1023 + digits - exp; i++)
                    {
                        tem_for_significant *= 5;
                    }
                    m_exp = new BigInteger(-(1023 + digits - exp));
                }

                // consider sign
                if (sign_bit == 1)
                    tem_for_significant = -tem_for_significant;

                m_base = tem_for_significant;

                // simplify the bignum format
                Simplify();
            }
        }

        // simplify the bignum class while it is valid & not undefined, and divisible by 10
        public void Simplify()
        {
            if (m_isUndefinded) return;

            // if base is zero, we make exponential equal to zero too
            if (m_base == 0)
            {
                m_exp = 0;
                return;
            }
                

            // remove the extra zeros and increase the exponential when base is divisible by 10
            while (m_base % 10 == 0 && m_base != 0)
            {
                m_base /= 10;
                m_exp++;
            }
        }

        // override this method to return a string
        public override string ToString()
        {
            Simplify(); // first simplify it to make sure format is good without trailing zeros

            if (IsUndefined)
                return "undefined";

            // get the sign and significants
            string sign = ""; // string for sign - or empty
            string digits = m_base.ToString();
            if (m_base < 0)
            {
                digits = digits.Substring(1);
                sign = "-";
            }
            
            // if exponential greater than 0, we add extra zeros to the base
            if(m_exp >= 0)
            {
                for (int i = 0; i < m_exp; i++)
                    digits += "0";
            }
            else if (m_exp >= (-digits.Length)) //if exponetial part is negative but less than the number of digits in base
            { // we insert '.' in the string
                digits = digits.Insert((int)(digits.Length + m_exp), ".");
            }
            else
            {// else, we insert proper amount of zeros before the base and insert '.' at last
                BigInteger loops = (-m_exp - digits.Length);
                for (int i = 0; i < loops; i++)
                    digits = "0" + digits;

                digits = "." + digits;
            }

            // build the final string with sign
            string result = sign + digits;
            return result;
        }

        // property for checking bignum is undefined or not
        public bool IsUndefined {
            get
            {
                return m_isUndefinded;
            }
        }

        // assistant function for copying from a bignum
        public void CopyFrom(BigNum value)
        {
            m_base = value.m_base;
            m_exp = value.m_exp;
            m_isUndefinded = value.m_isUndefinded;
        }
        
        // do addition
        public static BigNum operator +(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return new BigNum(0, 0, true);

            // copy base for inflation
            BigInteger lbase = lhs.m_base;
            BigInteger rbase = rhs.m_base;

            // inflate the base which has bigger exponential part, then return the new BigNum
            if (lhs.m_exp > rhs.m_exp)
            {
                for (int i = 0; i < lhs.m_exp - rhs.m_exp; i++)
                    lbase *= 10;

                return new BigNum(lbase + rbase, rhs.m_exp, false);
            }
            else
            {
                for (int i = 0; i < rhs.m_exp - lhs.m_exp; i++)
                    rbase *= 10;

                return new BigNum(lbase + rbase, lhs.m_exp, false);
            }
        }

        // do minus
        public static BigNum operator -(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return new BigNum(0, 0, true);

            BigInteger lbase = lhs.m_base;
            BigInteger rbase = rhs.m_base;

            // inflate the base which has bigger exponential part, then return the new BigNum
            if (lhs.m_exp > rhs.m_exp)
            {
                for (int i = 0; i < lhs.m_exp - rhs.m_exp; i++)
                    lbase *= 10;

                return new BigNum(lbase - rbase, rhs.m_exp, false);
            }
            else
            {
                for (int i = 0; i < rhs.m_exp - lhs.m_exp; i++)
                    rbase *= 10;

                return new BigNum(lbase - rbase, lhs.m_exp, false);
            }
        }

        // do multiplication, combining base and exponential
        public static BigNum operator *(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return new BigNum( 0, 0, true);

            return new BigNum(lhs.m_base * rhs.m_base, lhs.m_exp + rhs.m_exp, false);
        }

        // do the division
        public static BigNum operator /(BigNum lhs, BigNum rhs)
        {
            // return undefined is either is undefined or rhs is 0
            if(lhs.IsUndefined || rhs.IsUndefined || rhs.m_base == 0)
                return new BigNum(0, 0, true);

            BigInteger lbase = lhs.m_base;
            BigInteger rbase = rhs.m_base;
            BigInteger lexp = lhs.m_exp;

            // get the number of digits of lhs, if negative, then we need to minus one for the digits number 
            // like -100 has only 3 digits instead of string.length of 4
            int ldigits = lhs.m_base.ToString().Length;
            if (lhs.m_base < 0)
                ldigits--;

            // get the number of digits of rhs, if negative, then we need to minus one for the digits number 
            // like -100 has only 3 digits instead of string.length of 4
            int rdigits = rhs.m_base.ToString().Length;
            if (rhs.m_base < 0)
                rdigits--;

            // check the resolution, if lhs's base has insufficient diffrence between digits number
            // we then need to inflate lhs's base
            if(ldigits - rdigits < digits_resolution)
            {
                for(int i = 0; i < digits_resolution - (ldigits - rdigits); i++)
                {
                    // if lhs's base (in absolute value) less than rhs'base, to ensure digits resolution, we need to inflate once more. 
                    if (i == (rdigits - ldigits) && (lbase < 0 ? -lbase : lbase) < (rbase < 0 ? -rbase : rbase))
                    {
                        lbase *= 10;
                        lexp--;
                    }
                    // do inflate
                    lbase *= 10;
                    lexp--;
                }
            }
            return new BigNum(lbase / rbase, lexp - rhs.m_exp, false);
        }

        // check if two operands are equal
        public static bool operator ==(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            // remember to simplify the format before directly compare their base and exponential
            lhs.Simplify();
            rhs.Simplify();

            if (lhs.m_exp == rhs.m_exp && lhs.m_base == rhs.m_base)
                return true;

            return false;
        }

        // Notice: all the comparison below must return false if either of the operands is undefined
        // do == comparison
        public static bool operator !=(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            if (lhs == rhs)
                return false;
            else
                return true;
        }

        // A Equals implementation is recommended by Microsoft if == operator is overrided.
        public override bool Equals(object obj)
        {
            if (obj is BigNum)
            {
                return this == (obj as BigNum);
            }
            else
                return false;
        }

        // A GetHashCode implementation is recommended by Microsoft if == operator is overrided.
        public override int GetHashCode()
        {
            return m_base.GetHashCode() + m_exp.GetHashCode() + IsUndefined.GetHashCode();
        }


        // do > comparison
        public static bool operator >(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            // copy the base part for inflating
            BigInteger lbase = lhs.m_base;
            BigInteger rbase = rhs.m_base;

            // compare the exponent to see which base is going to be inflated before base comparison
            if (lhs.m_exp > rhs.m_exp)
            {
                for (int i = 0; i < lhs.m_exp - rhs.m_exp; i++)
                    lbase *= 10;

                return lbase > rbase;
            }
            else
            {
                for (int i = 0; i < rhs.m_exp - lhs.m_exp; i++)
                    rbase *= 10;

                return lbase > rbase;
            }
        }

        // do >= comparison
        public static bool operator >=(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            return lhs > rhs || lhs == rhs;
        }

        // do < comparison
        public static bool operator <(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            return !(lhs >= rhs);
        }

        // do <= comparison
        public static bool operator <=(BigNum lhs, BigNum rhs)
        {
            if (lhs.IsUndefined || rhs.IsUndefined)
                return false;

            return !(lhs > rhs);
        }

        // check if double.toString gives the correct result
        public static bool IsToStringCorrect(double value)
        {
            BigNum num1 = new BigNum(value, false);
            BigNum num2 = new BigNum(value, true);
            
            return num1 == num2;
        }
    }
}
