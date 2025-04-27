using System.Numerics;
using System.Security.Cryptography;

namespace DHKex;

public static class MillerRabinPrime {
    private static BigInteger NBitRandom(int bitLength) {
        var rng = RandomNumberGenerator.Create(); // create new RNG instance
        byte[] randomNumberBytes = new byte[bitLength / 8]; // create buffer to store number. for 1024 bits = 128 bytes
        rng.GetBytes(randomNumberBytes); // fill buffer with random bytes
        
        var result = new BigInteger(randomNumberBytes); // create new BigInteger and fill it with the data from the buffer
        result = BigInteger.Abs(result); // get absolute value of result, not sure if this is needed. Ensures the number is positive!
        
        return result;
    }

    private static BigInteger GetLowPrimeCandidate(int bitLength) {
        var iteration = 0; // maybe used later for metrics
        
        // loop until a candidate has been found
        while (true) {
            // generate a random number with n amount of bits
            BigInteger primeCandidate = NBitRandom(bitLength);
            
            // pre-computed prime list
            int[] primeList = [
                    2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
                    31, 37, 41, 43, 47, 53, 59, 61, 67,
                    71, 73, 79, 83, 89, 97, 101, 103,
                    107, 109, 113, 127, 131, 137, 139,
                    149, 151, 157, 163, 167, 173, 179,
                    181, 191, 193, 197, 199, 211, 223,
                    227, 229, 233, 239, 241, 251, 257,
                    263, 269, 271, 277, 281, 283, 293,
                    307, 311, 313, 317, 331, 337, 347, 349
            ];
            
            // check against each pre-computed prime
            foreach (int divisor in primeList) {
                // if primeCandidate / prime == 0 and prime * prime is less than primeCandidate the number is composite
                if (primeCandidate % divisor == 0 && divisor * divisor <= primeCandidate) {
                    break;
                }

                return primeCandidate; // return a prime candidate
            }
            
            iteration++;
        }
    }

    private static bool SingleTest(BigInteger primeCandidate, BigInteger a) {
        BigInteger n = primeCandidate; // for easier naming
        BigInteger nMinusOne = n - BigInteger.One;
        BigInteger exp = nMinusOne; // dont really know why the exponent needs to be n-1
        int s   = 0;

        // check if the lsb is 0, which means that the number is odd. Do this until we find (s, exp) such that n-1 = 2^s * d
        while (!(exp & BigInteger.One).IsZero) {
            exp >>= 1; // divide exp by 2 using a left shift assignment
            s++;
        }
        
        // Modular exponentiation (a^exp) % n
        BigInteger result = BigInteger.ModPow(a, exp, n);
        
        if (result == BigInteger.One || result == nMinusOne) return true; // if result is 1 or n-1 the number is probably prime?

        for (int r = 1; r < s; r++) {
            exp    <<= 1; // multiply by 2 using a right shift assignment
            result =   BigInteger.ModPow(a, exp, n);
            if (result == nMinusOne) {
                return true; // have to check why this is mathematically probably a prime
            }
        }

        return false; // the number is composite if all above doesnt return true
    }
    
    private static bool MillerRabinTest(BigInteger primeCandidate, int rounds = 40) {
        // loop through the test n amount of times
        for (int i = 0; i < rounds; i++) {
            // generate random a where a = 1 < a < n-1
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                byte[]     buffer; // initialize new byte buffer for the number generator
                BigInteger a; // initialize variable a
                BigInteger min = 2; // set to min value to 2, because a > 1
                BigInteger max = primeCandidate - BigInteger.One; // set max value to n-1 because a < n-1

                // loop until a <= 2 or a >= n-1
                do {
                    // get the minimum amount of bytes needed to create the buffer
                    int byteCount = (int)Math.Ceiling((primeCandidate.GetBitLength() + 7) / 8.0); // add 7 bits so that the (int) cast will round up correctly as Math.Ceiling returns a double
                                                                                                  // e.g. (16 + 7 ) = 23 bits / 8.0 = 2.875 so rounded it would be 3 bytes needed to store a 16 bit number in the buffer
                    buffer = new byte[byteCount]; // create the buffer with the correct size calculated in the step above
                    rng.GetBytes(buffer); // fill the buffer with random bytes
                    
                    // ensure that the generated number is positive
                    buffer[^1] |= 0x80; 
                    // ^1 is the same as "buffer.Length - 1" so the last byte in the array (MSB)
                    // bitwise OR assignment operator |= e.g. 1001 | 0101 = 1101
                    // 0x80 = 128 in decimal or 1000000 in binary
                    // so we ensure that the MSB of our last byte inside the array is always a 1
                    // the most significant bit of the most significant byte in an BigInteger value determines if a number is positive or negative

                    a = new BigInteger(buffer); // create a new BigInteger a from our buffer which has ensured to be a positive number

                    // safe guard, does the same as above. Could also use BigInteger.Abs(a) but is slower because this case basically never happens
                    if (a.Sign < 0) {
                        a = -a;
                    }
                } while (a <= min || a >= max);
                
                // run the test using the random a value and return false if the number is not prime
                if(!SingleTest(primeCandidate, a)) return false;
            }
        }
        
        // if no single round returned false, we can assume the number is a prime number
        return true;
    }
    public static BigInteger GetNBitPrime(int bitLength, int rounds = 40) {
        // loop until a prime has been found, this might take a while, so there should be a way to cancel it or smth
        while (true) {
            // get a prime candidate which has been checked already against pre-computed list of primes
            var primeCandidate = GetLowPrimeCandidate(bitLength);
            
            // test the candidate using Miller Rabin Primality Test using n amount of rounds
            if(MillerRabinTest(primeCandidate, rounds)) return primeCandidate; // returns possible prime as BigInteger
        }
    }
}