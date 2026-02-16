import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  KeyboardAvoidingView,
  Platform,
  ActivityIndicator,
  ScrollView,
} from 'react-native';
import { useRouter } from 'expo-router';
import { useAuth } from '@/contexts/AuthContext';
import { colors } from '@/styles/commonStyles';

export default function AuthScreen() {
  const router = useRouter();
  const { signInWithEmail, loading } = useAuth();
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async () => {
    if (!username || !password) {
      setError('Preencha todos os campos');
      return;
    }

    setError('');
    try {
      await signInWithEmail(username, password);
      router.replace('/(tabs)/');
    } catch (err: any) {
      setError(err.message || 'Erro ao autenticar');
    }
  };

  return (
    <KeyboardAvoidingView 
      style={styles.container} 
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
    >
      <ScrollView 
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.centerContainer}>
          {/* HEADER REESTILIZADO - ESTILO EMPILHADO */}
          <View style={styles.header}>
            <Text style={styles.desafioText}>desafio</Text>
            <Text style={styles.minervaText}>minerva</Text>
            <Text style={styles.foodsText}>foods</Text>
            
            <View style={styles.authorContainer}>
              <Text style={styles.authorText}>P H I L L I P E</Text>
              <Text style={styles.authorText}>R O G E R</Text>
              <Text style={styles.authorText}>S O U Z A</Text>
            </View>
          </View>

          <View style={styles.form}>
            <Text style={styles.label}>E-mail</Text>
            <TextInput
              style={styles.input}
              placeholder="Digite seu e-mail"
              placeholderTextColor="#bbb"
              value={username}
              onChangeText={setUsername}
              autoCapitalize="none"
            />

            <Text style={styles.label}>Senha</Text>
            <TextInput
              style={styles.input}
              placeholder="Digite sua senha"
              placeholderTextColor="#bbb"
              value={password}
              onChangeText={setPassword}
              secureTextEntry
              autoCapitalize="none"
            />

            {error ? <Text style={styles.errorText}>{error}</Text> : null}

            <TouchableOpacity 
              style={[styles.button, loading && { opacity: 0.7 }]} 
              onPress={handleLogin}
              disabled={loading}
              activeOpacity={0.8}
            >
              {loading ? (
                <ActivityIndicator color="#ffffff" />
              ) : (
                <Text style={styles.buttonText}>ENTRAR</Text>
              )}
            </TouchableOpacity>
          </View>
        </View>
        <View style={styles.authorContainer}>
              <Text style={styles.footerText}>P H I L L I P E @ 2 0 2 6</Text>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#ffffff',
  },
  scrollContent: {
    flexGrow: 1,
    justifyContent: 'center',
    paddingHorizontal: 40,
  },
  centerContainer: {
    width: '100%',
  },
  header: {
    alignItems: 'flex-start',
    marginBottom: 30,
  },
  desafioText: {
    fontSize: 58,
    fontWeight: '700',
    color: '#ececec',
    lineHeight: 50,
    marginLeft: -3,
  },
  minervaText: {
    fontSize: 52,
    fontWeight: '700',
    color: '#2b5373',
    lineHeight: 52,
  },
  foodsText: {
    fontSize: 46,
    fontWeight: '700',
    color: '#e84c53',
    marginTop: -8,
  },
  authorContainer: {
    marginTop: 10,
  },
  authorText: {
    fontSize: 9,
    color: '#bbb',
    letterSpacing: 6,
    lineHeight: 14,
    textTransform: 'uppercase',
  },
  form: {
    width: '100%',
    marginTop: 10,
  },
  label: {
    fontSize: 13,
    fontWeight: '600',
    color: '#444',
    marginBottom: 5,
  },
  input: {
    backgroundColor: '#fbfbfb',
    borderWidth: 1,
    borderColor: '#eee',
    borderRadius: 8,
    padding: 16,
    marginBottom: 15,
    fontSize: 15,
    color: '#333',
  },
  button: {
    backgroundColor: '#2b5373', // Azul Minerva
    borderRadius: 8,
    padding: 16,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 10,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
  },
  buttonText: {
    color: '#ffffff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  errorText: {
    color: '#e84c53',
    fontSize: 14,
    marginBottom: 12,
    textAlign: 'center',
    fontWeight: '500',
  },
  footerText: {
    fontSize: 9,
    color: '#bbb',
    letterSpacing: 6,
    lineHeight: 14,
    textTransform: 'uppercase',
    textAlign: 'center',
  },
});