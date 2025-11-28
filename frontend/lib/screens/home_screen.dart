import 'package:flutter/material.dart';

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
                children: [Text("GroupName"), Icon(Icons.arrow_drop_down_outlined)],
              ),
              onTap: () {
                print("Group clicked");
              },
            ),
          ],
        ),
        actions: [
          IconButton(
            tooltip: "More...",
            icon: Icon(Icons.more_vert),
            onPressed: () {
              print("Iconbutton");
              print("df");
            },
          ),
        ],
      ),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _selectedIndex,
        onTap: (newIndex) {
          setState(() {
            _selectedIndex = newIndex;
          });
        },
        items: [
          BottomNavigationBarItem(label: "Food", icon: Icon(Icons.fastfood)),
          BottomNavigationBarItem(label: "Settle Up", icon: Icon(Icons.compare_arrows)),
          BottomNavigationBarItem(label: "Group", icon: Icon(Icons.group)),
        ],
      ),
    );
  }
}
