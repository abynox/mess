import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';

class FoodView extends StatefulWidget {
  const FoodView({super.key});

  @override
  State<StatefulWidget> createState() => _FoodViewState();
}

class _FoodViewState extends State<FoodView> {
  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        SizedBox.expand(child: Text("Food view")),

        // FloatingActionButton
        Positioned(
          bottom: 16,
          right: 16,
          child: SafeArea(
            child: FloatingActionButton(
              onPressed: () {
                // todo: handle add food
                print("floating action");
              },
              child: const Icon(Icons.add),
            ),
          ),
        ),
      ],
    );
  }
}
