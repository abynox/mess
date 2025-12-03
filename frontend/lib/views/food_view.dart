import 'package:flutter/material.dart';
import 'package:mess/views/home_screen_view.dart';

class FoodView extends HomeScreenView {
  const FoodView({super.key});

  @override
  State<StatefulWidget> createState() => _FoodViewState();

  @override
  FloatingActionButton? buildFloatingActionButton(BuildContext context, VoidCallback rebuildHomeScreen) {
    return FloatingActionButton(
      onPressed: () {
        // todo: handle add food
        print("floating action");
        rebuildHomeScreen();
      },
      child: const Icon(Icons.add),
    );
  }
}

class _FoodViewState extends State<FoodView> {
  @override
  Widget build(BuildContext context) {
    print("Rebuild food view");
    return Stack(
      children: [
        // Column(
        //   children: [
        SizedBox.expand(child: Text("Food view")),

        ListTile(
          leading: Text("Leading"),
          title: Text("Title"),
          subtitle: Text("subtitle"),
          trailing: Text("trailing"),
        ),

        Card(
          child: ListTile(
            leading: Text("Leading"),
            title: Text("Title"),
            subtitle: Text("subtitle"),
            trailing: Text("trailing"),
          ),
        ),
        // ],
        // ),
      ],
    );
  }
}
