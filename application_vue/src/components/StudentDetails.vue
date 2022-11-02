<template>
  <div v-if="currentStudent.hashId" class="card bg-light p-3 edit-form">
    <h4>Dados do Estudante</h4>
    <form>
      <div class="form-group">
        <label for="nome">Nome <span class="text-danger">*</span></label>
        <input
          type="text"
          class="form-control"
          id="nome"
          v-model="currentStudent.nome"
          @keyup="currentStudent.nome.length < 3 ? null : keyupNome()"
          :class="{ 'is-invalid': !nomeValido }"
        />
        <div class="invalid-feedback">
          <span v-if="!nomeValido">Nome deve conter 3 ou mais caracteres.</span>
        </div>
      </div>
      <div class="form-group">
        <label for="CPF">CPF <span class="text-danger">*</span></label>
        <input
          type="text"
          class="form-control"
          id="CPF"
          maxlength="14"
          disabled
          v-model="currentStudent.cpf"
          @keyup="keyupCPF($event)"
          :class="{ 'is-invalid': !cpfValido }"
        />
        <div class="invalid-feedback">
          <span v-if="!cpfValido">CPF inválido.</span>
        </div>
      </div>
      <div class="form-group">
        <label for="email">E-mail <span class="text-danger">*</span></label>
        <input
          type="text"
          class="form-control"
          id="email"
          v-model="currentStudent.email"
          @keyup="!emailValido ? keyupEmail() : null"
          :class="{ 'is-invalid': !emailValido }"
        />
        <div class="invalid-feedback">
          <span v-if="!emailValido">E-mail inválido.</span>
        </div>
      </div>
      <div class="form-group">
        <label for="ra">RA</label>
        <input
          type="text"
          class="form-control"
          id="ra"
          disabled
          v-model="currentStudent.ra"
        />
      </div>
    </form>

    <div class="row pl-3 pr-3 justify-content-between">
      <button class="btn btn-danger mr-2" @click="handleDeleteStudent">
        Deletar
      </button>

      <button type="submit" class="btn btn-success" @click="updateStudent">
        Atualizar
      </button>
    </div>
  </div>

  <div v-else>
    <br />
    <p>Please click on a Student...</p>
  </div>
</template>
  
  <script lang="ts">
import { defineComponent } from "vue";
import RegistrationDataService from "@/services/RegistrationDataService";
import Student from "@/types/Student";
import ResponseData from "@/types/ResponseData";
import Swal from "sweetalert2/dist/sweetalert2.js";

const regexpEmail =
  /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

const swalWithBootstrapButtons = Swal.mixin({
  customClass: {
    confirmButton: "btn btn-success",
    cancelButton: "btn btn-danger mr-3",
  },
  buttonsStyling: false,
});

export default defineComponent({
  name: "student",
  data() {
    return {
      currentStudent: {} as Student,
      nomeValido: true,
      cpfValido: true,
      emailValido: true,
    };
  },
  methods: {
    getStudent(hashId: any) {
      RegistrationDataService.get(hashId)
        .then((response: ResponseData) => {
          this.currentStudent = response.data;
          console.log(response.data);
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    handleUpdateStudent() {
      Swal.fire({
        title: "Estudante Atualizado",
        icon: "success",
        confirmButtonText: "Ok",
      }).then(() => {
        this.$router.push({ name: "home" });
      });
    },

    updateStudent() {
      if (this.validate())
        RegistrationDataService.update(this.currentStudent)
          .then((response: ResponseData) => {
            console.log(response.data);
            this.handleUpdateStudent();
          })
          .catch((e: Error) => {
            console.log(e);
          });
    },

    handleDeleteStudent() {
      swalWithBootstrapButtons
        .fire({
          title: "Atenção",
          text: "Deseja deletar este estudante!",
          icon: "warning",
          showCancelButton: true,
          confirmButtonText: "Sim, Deletar",
          cancelButtonText: "Não, Cancelar!",
          reverseButtons: true,
        })
        .then(async (result) => {
          if (result.isConfirmed) {
            if (await this.deleteStudent()) {
              swalWithBootstrapButtons.fire(
                "Estudante deletado com sucesso!",
                "",
                "success"
              );
              this.$router.push({ name: "home" });
            } else {
              swalWithBootstrapButtons.fire(
                "Erro!",
                "Ocorreu um erro ao deletar o estudante!",
                "error"
              );
            }
          } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
          ) {
            swalWithBootstrapButtons.fire(
              "Operação cancelada",
              "Estudante não foi deletado",
              "error"
            );
          }
        });
    },

    deleteStudent(): Promise<boolean> {
      return RegistrationDataService.delete(this.currentStudent.hashId)
        .then((response: ResponseData) => {
          console.log(response.data);
          return true;
        })
        .catch((e: Error) => {
          console.log(e);
          return false;
        });
    },

    keyupCPF(v: any) {
      this.currentStudent.cpf = this.currentStudent.cpf.replace(/\D/g, "");
      this.currentStudent.cpf = this.currentStudent.cpf.replace(
        /(\d{3})(\d)/,
        "$1.$2"
      );
      this.currentStudent.cpf = this.currentStudent.cpf.replace(
        /(\d{3})(\d)/,
        "$1.$2"
      );
      this.currentStudent.cpf = this.currentStudent.cpf.replace(
        /(\d{3})(\d{1,2})$/,
        "$1-$2"
      );

      if (this.currentStudent.cpf.length == 14) this.cpfValido = true;
      if (this.currentStudent.cpf.length > 14) this.cpfValido = false;

      return this.currentStudent.cpf;
    },

    keyupNome() {
      this.currentStudent.nome.trim().length < 3
        ? (this.nomeValido = false)
        : (this.nomeValido = true);
    },

    keyupEmail() {
      !regexpEmail.test(this.currentStudent.email)
        ? (this.emailValido = false)
        : (this.emailValido = true);
    },

    validate(): boolean {
      this.keyupNome();
      this.keyupEmail();

      this.currentStudent.cpf.length != 14
        ? (this.cpfValido = false)
        : (this.cpfValido = true);

      return this.nomeValido && this.cpfValido && this.emailValido;
    },
  },
  mounted() {
    this.getStudent(this.$route.params.hash_id);
  },
});
</script>
  
  <style>
.edit-form {
  max-width: 300px;
  margin: auto;
}
</style>