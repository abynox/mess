import 'package:flutter/material.dart';
import 'package:mess/views/food_view.dart';
import 'package:mess/views/group_view.dart';
import 'package:mess/views/settle_up_view.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _selectedIndex = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Row(
          children: [
            Text("Group: "),
            InkWell(
              child: Row(
                mainAxisAlignment: MainAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [Text("GroupName"), const Icon(Icons.arrow_drop_down_outlined)],
              ),
              onTap: () {
                // todo: change group popup
                print("Group clicked");
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
