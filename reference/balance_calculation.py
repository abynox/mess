# By Abynox. Git blame is wrong!!!!

# member -> price in cent
payments = {
    "Member1": 1100,
    "Member2":  900,
    "Member3":    0,
    "Member4":    0,
}

# payments need to contain all members
total_members = len(payments)
total_cost = sum(payments.values())

# Member2 owes Member1 2 euros
debts = {
    "Member2": {
        "Member1": 200
    }
}


share = total_cost // total_members
remainder = total_cost % total_members
print("share", share, "remainder", remainder)

# Calc the balance for each member
balances = {
    member: payments[member] - share for member in payments
}

print("Balances", balances)

# add old debts
for debtor, creditors in debts.items():
    for creditor, amount in creditors.items():
        balances[debtor] = balances[debtor] - amount
        balances[creditor] = balances[creditor] + amount

print("Balances with debts", balances)

transactions = []

while True:
    # find creditor with the highest balance
    creditor = max(balances, key=lambda member: balances[member])
    # find debtor with the lowest balance
    debtor = min(balances, key=lambda member: balances[member])

    credit = balances[creditor]
    debit = balances[debtor]

    if credit <= 0 or debit >= 0:
        break

    # find the amount we can settle between creditor and debtor
    amount = min(credit, -debit)

    transactions.append((debtor, creditor, amount))

    balances[creditor] -= amount
    balances[debtor] += amount

print(transactions)
for debtor, creditor, amount in transactions:
    print(f"{debtor} -> {creditor}: {amount / 100}€")
print(f"The remainder is {remainder}")