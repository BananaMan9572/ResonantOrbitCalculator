﻿    using System;

    namespace ResonantMod
    {
        internal class ResonantOrbitCalculator
        {
            public float Periapsis { get; private set; }
            public float Apoapsis { get; private set; }
            public float Injection { get; private set; }
            private double smaResonant;

            public bool CalculateOrbit(CelestialBody body, float altitudeKm, int numberOfSats, out string errorMessage)
            {
                errorMessage = string.Empty;

                // Reset values in case of failure
                Periapsis = 0;
                Apoapsis = 0;
                Injection = 0;

                if (body == null)
                {
                    errorMessage = "No celestial body selected.";
                    return false;
                }

                if (altitudeKm < 0)
                {
                    errorMessage = "Altitude must be non-negative.";
                    return false;
                }

                if (numberOfSats < 3)
                {
                    errorMessage = "At least 3 satellites required.";
                    return false;
                }

                try
                {
                    double radius = body.Radius;
                    double rTarget = radius + (altitudeKm * 1000);
                    double gm = body.gravParameter;

                    double tTarget = 2 * Math.PI * Math.Sqrt(Math.Pow(rTarget, 3) / gm);
                    double tResonant = tTarget * (numberOfSats + 1) / numberOfSats;
                    smaResonant = Math.Pow((tResonant * tResonant) * gm / (4 * Math.PI * Math.PI), 1.0 / 3.0);

                    double rPeriapsis = rTarget;
                    double rApoapsis = 2 * smaResonant - rTarget;

                    if (rApoapsis <= rPeriapsis)
                    {
                        errorMessage = "Resonant orbit calculation failed. Try different parameters.";
                        return false;
                    }

                    // Set results
                    Periapsis = (float)((rPeriapsis - radius) / 1000);
                    Apoapsis = (float)((rApoapsis - radius) / 1000);

                    // Calculate injection ΔV
                    double vPeriapsis = Math.Sqrt(gm * (2 / rPeriapsis - 1 / smaResonant));
                    double vCircular = Math.Sqrt(gm / rPeriapsis);
                    Injection = (float)(vPeriapsis - vCircular);

                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Calculation error: {ex.Message}";
                    return false;
                }
            }
        }
    }
