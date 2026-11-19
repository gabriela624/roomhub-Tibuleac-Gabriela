# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| _(fill in)_ | _(fill in)_ | _(fill in)_ | _(fill in)_ |
R7 Services/BookingService.cs, în metoda de căutare a rezervărilor unei camere. Am modificat repository-ul astfel încât să returneze doar rezervările active. În acest fel, rezervările anulate nu mai blochează intervalul respectiv.