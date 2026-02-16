
// Toast Notification Component
import React, { useEffect } from 'react';
import { View, Text, StyleSheet } from 'react-native';
import Animated, { 
  useAnimatedStyle, 
  useSharedValue, 
  withSpring, 
  withDelay,
  runOnJS
} from 'react-native-reanimated';
import { colors } from '@/styles/commonStyles';

interface ToastProps {
  visible: boolean;
  message: string;
  type?: 'success' | 'error' | 'info';
  onHide: () => void;
}

export function Toast({ visible, message, type = 'info', onHide }: ToastProps) {
  const translateY = useSharedValue(-100);

  useEffect(() => {
    if (visible) {
      console.log('Toast: Showing message', message);
      translateY.value = withSpring(0, { damping: 15 });
      
      const timer = setTimeout(() => {
        translateY.value = withSpring(-100, { damping: 15 }, () => {
          runOnJS(onHide)();
        });
      }, 3000);

      return () => clearTimeout(timer);
    }
  }, [visible, message, onHide, translateY]);

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: translateY.value }],
  }));

  const backgroundColor = type === 'success' 
    ? colors.success 
    : type === 'error' 
    ? colors.error 
    : colors.info;

  if (!visible) return null;

  return (
    <Animated.View style={[styles.container, animatedStyle, { backgroundColor }]}>
      <Text style={styles.message}>{message}</Text>
    </Animated.View>
  );
}

const styles = StyleSheet.create({
  container: {
    position: 'absolute',
    top: 60,
    left: 20,
    right: 20,
    padding: 16,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    zIndex: 9999,
  },
  message: {
    fontSize: 14,
    fontWeight: '600',
    color: '#ffffff',
    textAlign: 'center',
  },
});
