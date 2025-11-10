<h2>The new implementation performs more than 4,100 times faster than the previous version: https://github.com/dave95995/SudokuSolver/tree/master</h2>

This project implements a Sudoku solver using a Constraint Satisfaction Problem (CSP) framework inspired by Artificial Intelligence: A Modern Approach (Russell & Norvig).
Each Sudoku cell is represented as a variable (Unit) with a domain of possible values (1–9) and constraints linking it to other cells in the same row, column, and box.

The solver combines two core AI techniques:

Constraint Propagation – Logical pattern checks (e.g., Single Value, Only Candidate, Box Pairs) reduce possible values before search, similar to forward checking and arc consistency.
Backtracking Search – A recursive depth-first search that assigns values consistent with all constraints, based directly on AIMA’s Backtracking-Search algorithm.

Artificial Intelligence: A Modern Approach (Russell & Norvig) Chapter 5
<img width="722" height="479" alt="Skärmbild 2025-11-10 130225" src="https://github.com/user-attachments/assets/a8937718-7ca5-4915-9f40-875b522e29c8" />
<img width="700" height="516" alt="Skärmbild 2025-11-10 130209" src="https://github.com/user-attachments/assets/a35f6948-f5db-419f-a67b-3470a9bef284" />
