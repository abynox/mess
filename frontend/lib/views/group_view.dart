import 'package:flutter/material.dart';
import 'package:mess/views/home_screen_view.dart';

class GroupView extends HomeScreenView {
  const GroupView({super.key});

  @override
  State<StatefulWidget> createState() => _GroupViewState();

  @override
  FloatingActionButton? buildFloatingActionButton(BuildContext context, VoidCallback rebuildHomeScreen) {
    return null;
  }
}

class _GroupViewState extends State<GroupView> {
  @override
  Widget build(BuildContext context) {
    return SizedBox.expand(child: Text("Group view"));
  }
}
