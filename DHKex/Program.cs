namespace DHKex;

public class DHKexMain {
    private static void Main(string[] args) {
        Console.WriteLine("Cores count: " + Environment.ProcessorCount);
        // Diffie-Hellman key exchange flow
        // Public parameter operations:
        // 1. Generate p value, p is prime. done
        // 1.1 Using the Rabin Miller Primality Test, results in a number which has a probability to be a prime number. done
        // 1.1.1 Preselect random number with desired bit length. done
        // 1.1.2 Ensure chosen number is not divisible by the first few hundred pre-computed primes. done
        // 1.1.3 Apply n amount Rabin Miller Primality Test iterations. done
        var bitLength    = 32;
        var randomNumber = MillerRabinPrime.GetNBitPrimeParallel(bitLength, 40, Environment.ProcessorCount);
        Console.WriteLine($"Random {bitLength} bit number: {randomNumber}");


        // 2. Generate g value, g is primitive root modulo
        // 2.1 Euler Totient Function phi = n-1
        // 2.2 Find all prime factors of hi
        // 2.3
        // A = g^a (mod p)

        // Private parameter operations:
        // 1. Generate private value a (secret key) for participant 1
        // 2. Generate private value b (secret key) for participant 2

        // Math operations:
        // 3. Combine secret key a with public values p & g resulting in value A (public key) for participant 1
        // 4. Combine secret key b with public values p & g resulting in value B (public key) for participant 2
        // 5. Transfer participants 1 public key A to participant B
        // 6. Transfer participants 2 public key B to participant A

        // Calculate shared secret
        // 7. Participant 1 combines public key B with secret key a
        // 8. Participant 2 combines public key A with secret key b
    }
}