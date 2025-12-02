import 'package:flutter/material.dart';

class ThemeProvider extends ChangeNotifier {
  ThemeMode? _themeMode;

  ThemeMode getThemeMode() {
    // todo: load from settings
    _themeMode ??= ThemeMode.system;
    return _themeMode!;
  }

  void setThemeMode(ThemeMode themeMode){
    // todo: save Theme
    _themeMode = themeMode;
    notifyListeners();
  }
}
