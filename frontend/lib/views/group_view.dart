import 'package:flutter/widgets.dart';

class GroupView extends StatefulWidget {
  const GroupView({super.key});

  @override
  State<StatefulWidget> createState() => _GroupViewState();
}

class _GroupViewState extends State<GroupView> {
  @override
  Widget build(BuildContext context) {
    return SizedBox.expand(child: Text("Group view"));
  }
}
