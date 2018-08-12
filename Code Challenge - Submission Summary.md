# Dexcom Code Challenge -- Crunching Large Data Sets
Submission by Jesse Ferguson 2018/08/12
-------------------------------------------------------

## BACKGROUND:

The included C# code sample calculates the Incremental Interquartile Mean of a large data set.  The data.txt file contains 100,000 integer values between 0 and 600.  The given code calculates the Interquartile Mean of the first 4 values in the data set, then the first 5 values, then the first 6 values, etc, until it has calculated the IQM of all 100,000 values (99997 IQM calculations total).

I've refactored the code so it's more readable, more extendable, more testable, and considerably more performant.

## MORE READABLE:

The original code sample had all the code in the `Program.Main()` method, with all kinds of unintelligible variable names, like q, i, c, and ys. I've moved the actual calculations to a separate static class, along with extracting the rest of the logic into a separate manager class, with clearly delineated regions and much more human-readable variable names. I've also inserted comments explaining what the code is doing anywhere it's not completely self-evident. Now the `Program.Main()` method isn't doing anything except setting initial values, instantiating the manager, telling it to run, and reporting the output. This also allows the class to be used in other places, potentially by other services if needs be.

## MORE EXTENDABLE

In the original sample, everything was hard-coded, so changing values would be difficult, time-consuming, and error-prone. After my refactor, users can dynamically set:
 * **The data input file**
 * **The IQM calculation method** (more on that below)
 * **The data insertion method** (more on that below, too)
 * **The HandleResultsMethod:** The user can specify how you want to handle the incremental results. The default is to write the results out to the console, but you can write them to a file instead, or add them to an in-memory list, or whatever they need to do.
 
 If, in the future, we'd like to add additional calculation or insertion methods, we can.
 
 ## MORE TESTABLE
 
 The extensibility improvements mentioned above, along with extracting the computation out to a separate class, allowed me to implement robust unit tests. I can verify the results of each calculation by outputting the results into an in-memory dictionary, then comparing the results to an expected set of values.
 
 ## CONSIDERABLY MORE PERFORMANT
 
 The biggest cause of slowdown in the original code sample was the sorting. Every time a new value was read in from the data set, it was added to the end of the list, then the entire list was sorted. Since the entire array is nearly already sorted, this didn't take as long as it generally does, but you're still looking at _n!_ comparisons every time a new record is added.
 
By replacing the sort with an in-line insertion, I'm essentially skipping all the irrelevant, redundant sort comparisons. In initial test runs this change alone resulted in a more than 60% reduction in runtime.

The second optimization came in the form of replacing the iterator traversal with more simple summation algorithm, and some minor refactors to simplify the algorithm itself.

It's also worth noting that the modified method isn't quite the same as a standard Interquartile Mean, in that it accounts for a fraction of the edges when the total dataset isn't divisible by 4. I've included the standard method here, in addition to the original, unaltered method originally presented in the code sample, and my refactor of said method, which is both more readable and slightly more performant than the original.

**TOTAL PEFORMANCE GAIN:** As mentioned above, replacing the "Add and sort" insertion method with one that inserts in place reduced runtime by nearly 60%. The fully optimized algorithm reduces runtime by 81%, and completes in about 30 seconds. 