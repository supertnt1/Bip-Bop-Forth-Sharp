Bip-Bop-Forth-Sharp
===================

A forth interpreter created in C#


Currently implemented:

+,-,/,*

dup - duplicate top item on stack

create - create a variable at the next available space

! - store second number on stack at address directed with number on top of stack

@ - fetch number from memory address using number at top of stack and put the result on stack

:; define new words

. print top of stack to terminal


example code:

  : squr dup * ;
  
  5 squr .
  
  
