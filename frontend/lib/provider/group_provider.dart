import 'dart:collection';

import 'package:flutter/widgets.dart';
import 'package:mess/models/group.dart';

class GroupProvider extends ChangeNotifier {
  final List<Group> _groups = List.empty();
  Group? _activeGroup;

  UnmodifiableListView<Group> get groups => UnmodifiableListView(_groups);
}
