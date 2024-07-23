# Dinner Kill Points

The use case of this software is to help a group of friends who often go to restaurants together
to efficiently split the bill. By having one person each time pay the bill, there is no hassle with
the restaurant. The debt is then tracked and the next time the most indebted person pays. This system
enables easy bill splitting at the restaurant while also ensuring over time bills are split fairly.

In practice I have found that this debt tracking only remains equatable if you have dinner frequently
enough to make the debt not stay with one person for too long. So recently I've mostly used the bill
splitting feature to split bills and then be immediately paid back.

## Interesting features

### Bill Splitter

The bill splitting logic live in the [`BillSplitter`](src/DkpLib/BillSplitter.cs) class. This
bill splitter has some interesting features:

- Obnoxiously fair and accurate bill splitting. It tracks debt in terms of pennies using floating
  point. Once all debts are allocated, it distributes the sub-penny debts fairly to make everything
  add up.
- Support for items bought by person or shared with the party.
- Distributes tax, tip, and group discounts proportionally to each person's spend amount.
- Supports two styles of having one or more people not pay for their meals, for the case of
  birthdays or other circumstances. In the case of `AddFreeLoader` function the person pays nothing.
  In the case of the `AddFremontBirthday` function, the person does not payform their own meal but
  does contribute to the cost of other people to whom the `AddFremontBirthday` is used.

### Debt Graph

When calculating who is owed money, the debts can be represented as a graph. Often this graph has
cycles. If debts were repaid along these cycles, eventually the money would return to the original
payer. The [`DebtGraph`](src/DkpWeb/Util/DebtGraph.cs) class finds these cycles and subtracts their
debt out. This leaves an acrylic graph of debts that easily actionable by the debtors.

## About the name

It is an allusion to
[Dragon Kill Points](https://en.wikipedia.org/wiki/Dragon_kill_points),
a system used in World of Warcraft and similar games to
decide who gets the loot from the dragon.
