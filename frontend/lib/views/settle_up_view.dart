import 'package:flutter/widgets.dart';

class SettleUpView extends StatefulWidget {
  const SettleUpView({super.key});

  @override
  State<StatefulWidget> createState() => _SettleUpState();
}

class _SettleUpState extends State<SettleUpView> {
  @override
  Widget build(BuildContext context) {
    return SizedBox.expand(child: Text("Settle up view"));
  }
}
