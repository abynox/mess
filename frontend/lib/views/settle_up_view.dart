import 'package:flutter/material.dart';
import 'package:mess/views/home_screen_view.dart';

class SettleUpView extends HomeScreenView {
  const SettleUpView({super.key});

  @override
  State<StatefulWidget> createState() => _SettleUpState();

  @override
  FloatingActionButton? buildFloatingActionButton(BuildContext context, VoidCallback rebuildHomeScreen) {
    return null;
  }
}

class _SettleUpState extends State<SettleUpView> {
  @override
  Widget build(BuildContext context) {
    return SizedBox.expand(child: Text("Settle up view"));
  }
}
