import FontAwesome from '@expo/vector-icons/FontAwesome';
import { useAuth } from "@/contexts/AuthContext";
import { ConfirmModal } from "@/components/ConfirmModal";
import { useRouter } from "expo-router";
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, ActivityIndicator, Dimensions } from "react-native";
import React, { useState } from "react";
import { colors } from "@/styles/commonStyles";
import { SafeAreaView } from "react-native-safe-area-context";
import { LogOut, User as UserIcon, ChevronRight } from "lucide-react-native";

const { width } = Dimensions.get('window');

export default function ProfileScreen() {
  const router = useRouter();
  const { user, signOut, loading } = useAuth();
  const [showLogoutModal, setShowLogoutModal] = useState(false);
  
  const handleLogout = async () => {
    setShowLogoutModal(false);
    await signOut();
    router.replace('/auth');
  };

  const userName = user?.name || user?.email || 'Usuário';
  const userEmail = user?.email || '';

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <ScrollView 
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* HEADER - Visual mais limpo */}
        <View style={styles.header}>
          <View style={styles.avatarWrapper}>
            <View style={styles.avatarContainer}>
              <UserIcon size={width * 0.12} color={colors.primary} />
            </View>
          </View>
          <Text style={styles.userName} numberOfLines={1}>{userName}</Text>
          {userEmail ? (
            <Text style={styles.userEmail} numberOfLines={1}>{userEmail}</Text>
          ) : null}
        </View>

        {/* SECTION - Info do App */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Aplicação</Text>
          
          <View style={styles.infoCard}>
            <Text style={styles.infoLabel}>Nome</Text>
            <View style={styles.valueContainer}>
              <Text style={styles.infoValue} numberOfLines={1}>Desafio Minerva Foods</Text>
            </View>
          </View>

          <View style={styles.infoCard}>
            <Text style={styles.infoLabel}>Versão</Text>
            <Text style={styles.infoValue}>1.0.0</Text>
          </View>
        </View>

        {/* BOTÃO SAIR - Mais robusto */}
        <TouchableOpacity
          style={styles.logoutButton}
          onPress={() => setShowLogoutModal(true)}
          disabled={loading}
          activeOpacity={0.8}
        >
          {loading ? (
            <ActivityIndicator color="#ffffff" />
          ) : (
            <>
              <LogOut size={20} color="#ffffff" />
              <Text style={styles.logoutButtonText}>Encerrar Sessão</Text>
            </>
          )}
        </TouchableOpacity>

        {/* FOOTER - Com espaçamento dinâmico */}
        <View style={styles.footer}>
          <View style={styles.divider} />
          <Text style={styles.footerText}>DESENVOLVIDO POR</Text>
          <Text style={styles.authorName}>PHILLIPE ROGER SOUZA</Text>
        </View>
      </ScrollView>

      <ConfirmModal
        visible={showLogoutModal}
        title="Sair da Conta"
        message="Tem certeza que deseja encerrar sua sessão atual?"
        confirmText="Sair"
        cancelText="Cancelar"
        onConfirm={handleLogout}
        onCancel={() => setShowLogoutModal(false)}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F4F7FA', // Um fundo levemente azulado/cinza fica mais moderno
    zIndex: 1
  },
  scrollContent: {
    paddingHorizontal: 24,
    paddingBottom: 40,
  },
  header: {
    alignItems: 'center',
    marginTop: 30,
    marginBottom: 40,
  },
  avatarWrapper: {
    padding: 8,
    borderRadius: 60,
    backgroundColor: '#fff',
    elevation: 4,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    marginBottom: 20,
  },
  avatarContainer: {
    width: width * 0.25,
    height: width * 0.25,
    borderRadius: (width * 0.25) / 2,
    backgroundColor: '#E8EFF5',
    alignItems: 'center',
    justifyContent: 'center',
  },
  userName: {
    fontSize: 22,
    fontWeight: '800',
    color: '#1A1C1E',
    textAlign: 'center',
    width: '100%',
  },
  userEmail: {
    fontSize: 14,
    color: '#6C757D',
    marginTop: 4,
    textAlign: 'center',
  },
  section: {
    marginBottom: 30,
  },
  sectionTitle: {
    fontSize: 13,
    fontWeight: '700',
    color: '#ACB5BD',
    marginBottom: 12,
    textTransform: 'uppercase',
    letterSpacing: 1.2,
  },
  infoCard: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 18,
    marginBottom: 12,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 5,
  },
  infoLabel: {
    fontSize: 15,
    fontWeight: '600',
    color: '#495057',
    flex: 1,
  },
  valueContainer: {
    flex: 2,
    alignItems: 'flex-end',
  },
  infoValue: {
    fontSize: 15,
    color: '#6C757D',
    fontWeight: '500',
  },
  logoutButton: {
    backgroundColor: '#FF4D4D',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    gap: 10,
    height: 56,
    borderRadius: 16,
    marginTop: 10,
    elevation: 4,
    shadowColor: '#FF4D4D',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
  },
  logoutButtonText: {
    color: '#ffffff',
    fontSize: 16,
    fontWeight: '700',
  },
  footer: {
    alignItems: 'center',
    marginTop: 50,
    marginBottom: 20, // Garante que não cole na borda inferior
  },
  divider: {
    width: 40,
    height: 2,
    backgroundColor: '#DEE2E6',
    marginBottom: 10,
  },
  footerText: {
    fontSize: 10,
    color: '#ACB5BD',
    fontWeight: '600',
    letterSpacing: 1, // Reduzi o espaçamento para evitar quebras em telas pequenas
  },
  authorName: {
    fontSize: 12,
    color: '#6C757D',
    fontWeight: 'bold',
    marginTop: 4,
  },
});