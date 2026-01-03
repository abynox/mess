using Mess.Data;

namespace Mess.Bank;

public class Banker
{
    public static Balance GetOrCreateBalance(Group group, Member member1, Member member2, AppDatabaseContext db)
    {
        //               2              3  -> true
        bool swapped = member2.Id <= member1.Id; // Check whether member1 and member2 need to be swapped
        Member payer = swapped ? member1 : member2; // member with higher id is the payer
        Member payee =  swapped ? member2 : member1; // member with lower id is the payee
        Balance? balance = db.Balances.FirstOrDefault(x => x.GroupId == group.Id && x.PayerId == payer.Id && x.PayeeId == payee.Id);
        if (balance == null)
        {
            balance = new Balance { GroupId = group.Id, PayerId = payer.Id, PayeeId = payee.Id };
            db.Balances.Add(balance);
        }
        balance.IsSwapped = swapped;
        return balance;
    }
    
    public static bool ProcessEntryPayments(Entry entry, AppDatabaseContext db)
    {
        decimal personCount = entry.GetTotalPersonCount();
        foreach (Participant payee in entry.Participants)
        {
            if (payee.PaidAmount == 0) continue;
            decimal ppp = payee.PaidAmount / personCount;
            foreach (Participant payer in entry.Participants)
            {
                Balance b = GetOrCreateBalance(entry.Group, payer.Member, payee.Member, db);
                b.Add(payer.GetPersonCount() * ppp);
            }
        }

        db.SaveChanges();
        return true;
    }
}