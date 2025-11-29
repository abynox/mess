import 'package:flutter/material.dart';
import 'package:mess/provider/group_provider.dart';
import 'package:mess/views/food_view.dart';
import 'package:mess/views/group_view.dart';
import 'package:mess/views/settle_up_view.dart';
import 'package:mess/widgets/group_modal_bottom_sheet.dart';
import 'package:provider/provider.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => context.read<GroupProvider>().loadGroups());
  }

  @override
  Widget build(BuildContext context) {
    final groupProvider = context.watch<GroupProvider>();

    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Row(
          children: [
            Text("Active group: "),
            InkWell(
              child: Row(
                mainAxisAlignment: MainAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(groupProvider.activeGroup != null ? groupProvider.activeGroup!.name : "No Group"),
                  const Icon(Icons.arrow_drop_down_outlined),
                ],
              ),
              onTap: () {
                // todo: change group popup
                print("Group clicked");
                showModalBottomSheet(
                  context: context,
                  isScrollControlled: true,
                  backgroundColor: Theme.of(context).colorScheme.surface,
                  builder: (context) => GroupModalBottomSheet(),
                );
              },
            ),
          ],
        ),
        actions: [
          IconButton(
            tooltip: "More...",
            icon: const Icon(Icons.more_vert),
            onPressed: () {
              // todo: show overflow menu
              print("Iconbutton");
            },
          ),
        ],
      ),
      body: [FoodView(), SettleUpView(), GroupView()][_selectedIndex],
      bottomNavigationBar: NavigationBar(
        selectedIndex: _selectedIndex,
        onDestinationSelected: (newIndex) => setState(() {
          _selectedIndex = newIndex;
        }),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.fastfood), label: "Food"),
          NavigationDestination(icon: Icon(Icons.compare_arrows), label: "Settle Up"),
          NavigationDestination(icon: Icon(Icons.group), label: "Group"),
        ],
      ),
    );
  }
}
